using MediatR;
using Microsoft.AspNetCore.Identity;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Domain.Entities.Identity;
using Sos.Core.Domain.Enums;
using Sos.Shared.Infrastructure.Services;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Commands;

// ── CreateOrganization ────────────────────────────────────────────────────────
public record CreateOrganizationCommand(
    string           NameUz,
    string           NameRu,
    string           Slug,
    OwnershipType?   Ownership = null,
    Guid?            OrgTypeId = null,
    string?           NameEn      = null,
    string?           NameUzCyrl = null,
    string?           NameKk      = null,
    string?           Code        = null,
    string?           Tin         = null,
    string?           Okonx       = null,
    string?           Oked        = null
) : IRequest<Result<Guid>>;

public class CreateOrganizationHandler(
    IOrganizationRepository repo,
    ICurrentContext         context,
    IUserRepository         userRepo,
    IEmployeeRepository     empRepo,
    UserManager<User>       userManager)
    : IRequestHandler<CreateOrganizationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrganizationCommand cmd, CancellationToken ct)
    {
        if (context.UserId is null)
            return Result.Failure<Guid>("Foydalanuvchi autentifikatsiyadan o'tmagan.");

        if (await repo.SlugExistsAsync(cmd.Slug, ct))
            return Result.Conflict<Guid, Organization>(cmd.Slug);

        if (cmd.Code is not null && await repo.CodeExistsAsync(cmd.Code, ct))
            return Result.Conflict<Guid, Organization>(cmd.Code);

        // Only SuperAdmin may create System organizations
        if (cmd.Ownership == OwnershipType.System && context.Role != nameof(UserRoles.SuperAdmin))
            return Result.Failure<Guid>("System turi tashkilotni faqat SuperAdmin yaratishi mumkin.");

        var org = Organization.Create(
            Guid.NewGuid(), cmd.NameUz, cmd.NameRu, cmd.Slug,
            context.UserId.Value, cmd.NameEn, cmd.NameUzCyrl, cmd.NameKk);

        org.Code      = cmd.Code;
        org.Tin       = cmd.Tin;
        org.Okonx     = cmd.Okonx;
        org.Oked      = cmd.Oked;
        if (cmd.Ownership.HasValue) org.SetType(cmd.Ownership.Value);
        org.OrgTypeId = cmd.OrgTypeId;

        await repo.AddAsync(org, ct);

        // Assign creating user to the new organization and ensure they have an employee record
        var user = await userRepo.GetByIdAsync(context.UserId.Value, ct);
        if (user is not null)
        {
            user.SetOrganization(org.Id);
            await userRepo.SaveChangesAsync(ct);

            // Give owner StoreAdmin role so they can manage their org
            await userManager.AddToRoleAsync(user, UserRoles.StoreAdmin.ToString());

            // Create an Employee entry for the user if missing
            if (!await empRepo.ExistsByUserIdAsync(user.Id, ct))
            {
                var names = (user.UserName ?? "").Split('.', 2, StringSplitOptions.RemoveEmptyEntries);
                var first = names.Length > 0 ? names[0] : user.UserName ?? "";
                var last  = names.Length > 1 ? names[1] : string.Empty;

                var emp = Employee.Create(user.Id, first, last);
                emp.OrganizationId = org.Id;
                await empRepo.AddAsync(emp, ct);
            }
        }

        return Result.Success(org.Id);
    }
}

// ── UpdateOrganizationNames ───────────────────────────────────────────────────
public record UpdateOrganizationNamesCommand(
    Guid    OrganizationId,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzCyrl = null,
    string? NameKk     = null
) : IRequest<Result>;

public class UpdateOrganizationNamesHandler(IOrganizationRepository repo, ICurrentContext context)
    : IRequestHandler<UpdateOrganizationNamesCommand, Result>
{
    public async Task<Result> Handle(UpdateOrganizationNamesCommand cmd, CancellationToken ct)
    {
        var org = await repo.GetByIdAsync(cmd.OrganizationId, ct);
        if (org is null) return Result.NotFound<Organization>(cmd.OrganizationId);
        if (org.OwnerUserId != context.UserId) return Result.Failure("Faqat egasi o'zgartirishi mumkin.");

        org.UpdateNames(cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.NameUzCyrl, cmd.NameKk);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── UpdateOrganization ────────────────────────────────────────────────────────
public record UpdateOrganizationCommand(
    Guid               OrganizationId,
    string?            Code    = null,
    string?            Tin     = null,
    string?            Okonx   = null,
    string?            Oked    = null,
    bool               IsTest  = false
) : IRequest<Result>;

public class UpdateOrganizationHandler(IOrganizationRepository repo, ICurrentContext context)
    : IRequestHandler<UpdateOrganizationCommand, Result>
{
    public async Task<Result> Handle(UpdateOrganizationCommand cmd, CancellationToken ct)
    {
        var org = await repo.GetByIdAsync(cmd.OrganizationId, ct);
        if (org is null) return Result.NotFound<Organization>(cmd.OrganizationId);
        if (org.OwnerUserId != context.UserId) return Result.Failure("Faqat egasi o'zgartirishi mumkin.");

        org.Code   = cmd.Code;
        org.Tin    = cmd.Tin;
        org.Okonx  = cmd.Okonx;
        org.Oked   = cmd.Oked;
        org.IsTest = cmd.IsTest;

        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── SetOrganizationParent ─────────────────────────────────────────────────────
public record SetOrganizationParentCommand(Guid OrganizationId, Guid? ParentId) : IRequest<Result>;

public class SetOrganizationParentHandler(IOrganizationRepository repo, ICurrentContext context)
    : IRequestHandler<SetOrganizationParentCommand, Result>
{
    public async Task<Result> Handle(SetOrganizationParentCommand cmd, CancellationToken ct)
    {
        var org = await repo.GetByIdAsync(cmd.OrganizationId, ct);
        if (org is null) return Result.NotFound<Organization>(cmd.OrganizationId);
        if (org.OwnerUserId != context.UserId) return Result.Failure("Faqat egasi o'zgartirishi mumkin.");

        if (cmd.ParentId.HasValue)
        {
            var parent = await repo.GetByIdAsync(cmd.ParentId.Value, ct);
            if (parent is null) return Result.NotFound<Organization>(cmd.ParentId.Value);
            org.SetParent(cmd.ParentId.Value);
        }
        else { org.RemoveParent(); }

        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── ToggleOrganizationStatus ──────────────────────────────────────────────────
public record ToggleOrganizationStatusCommand(Guid OrganizationId, bool IsActive) : IRequest<Result>;

public class ToggleOrganizationStatusHandler(IOrganizationRepository repo, ICurrentContext context)
    : IRequestHandler<ToggleOrganizationStatusCommand, Result>
{
    public async Task<Result> Handle(ToggleOrganizationStatusCommand cmd, CancellationToken ct)
    {
        var org = await repo.GetByIdAsync(cmd.OrganizationId, ct);
        if (org is null) return Result.NotFound<Organization>(cmd.OrganizationId);
        if (org.OwnerUserId != context.UserId) return Result.Failure("Faqat egasi o'zgartirishi mumkin.");

        if (cmd.IsActive) org.Activate(); else org.Deactivate();
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── DeleteOrganization ────────────────────────────────────────────────────────
public record DeleteOrganizationCommand(Guid OrganizationId) : IRequest<Result>;

public class DeleteOrganizationHandler(IOrganizationRepository repo, ICurrentContext context)
    : IRequestHandler<DeleteOrganizationCommand, Result>
{
    public async Task<Result> Handle(DeleteOrganizationCommand cmd, CancellationToken ct)
    {
        var org = await repo.GetByIdAsync(cmd.OrganizationId, ct);
        if (org is null) return Result.NotFound<Organization>(cmd.OrganizationId);
        if (org.OwnerUserId != context.UserId) return Result.Failure("Faqat egasi o'chirishi mumkin.");

        org.SoftDelete(context.UserId);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── ClassifyOrganization ──────────────────────────────────────────────────────
/// <summary>
/// Tashkilot tur (System/Customer) va darajasini (Root/Chain/Store) o'rnatish.
/// Biznes qoidalar:
///   • Store darajasidagi tashkilot ParentId ga ega bo'lishi shart.
///   • System turi faqat Root yoki Chain darajasida bo'la oladi.
///   • Customer turi har qanday darajada bo'lishi mumkin.
/// </summary>
public record ClassifyOrganizationCommand(
    Guid              OrganizationId,
    OwnershipType     Ownership,
    OrganizationLevel Level
) : IRequest<Result>;

public class ClassifyOrganizationHandler(IOrganizationRepository repo, ICurrentContext context)
    : IRequestHandler<ClassifyOrganizationCommand, Result>
{
    public async Task<Result> Handle(ClassifyOrganizationCommand cmd, CancellationToken ct)
    {
        var org = await repo.GetByIdAsync(cmd.OrganizationId, ct);
        if (org is null) return Result.NotFound<Organization>(cmd.OrganizationId);

        // Biznes qoida 1: Store — parent bo'lishi shart
        if (cmd.Level == OrganizationLevel.Store && org.ParentId is null)
            return Result.Failure("Store darajasidagi tashkilot chain/root ga bog'liq bo'lishi shart.");

        // Biznes qoida 2: System faqat Root yoki Chain bo'lishi mumkin
        if (cmd.Ownership == OwnershipType.System && cmd.Level == OrganizationLevel.Store)
            return Result.Failure("System turi do'kon (Store) darajasida bo'la olmaydi.");

        org.Classify(cmd.Ownership, cmd.Level);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── AddMember ─────────────────────────────────────────────────────────────────
public record AddMemberCommand(Guid OrganizationId, Guid UserId, OrganizationRole Role = OrganizationRole.Member)
    : IRequest<Result<Guid>>;

public class AddMemberHandler(IOrganizationRepository repo, ICurrentContext context)
    : IRequestHandler<AddMemberCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddMemberCommand cmd, CancellationToken ct)
    {
        var org = await repo.GetByIdAsync(cmd.OrganizationId, ct);
        if (org is null) return Result.NotFound<Guid, Organization>(cmd.OrganizationId);
        if (org.OwnerUserId != context.UserId) return Result.Failure<Guid>("Faqat egasi a'zo qo'sha oladi.");

        var member = org.AddMember(cmd.UserId, cmd.Role);
        await repo.SaveChangesAsync(ct);
        return Result.Success(member.Id);
    }
}

// ── RemoveMember ──────────────────────────────────────────────────────────────
public record RemoveMemberCommand(Guid OrganizationId, Guid UserId) : IRequest<Result>;

public class RemoveMemberHandler(IOrganizationRepository repo, ICurrentContext context)
    : IRequestHandler<RemoveMemberCommand, Result>
{
    public async Task<Result> Handle(RemoveMemberCommand cmd, CancellationToken ct)
    {
        var org = await repo.GetByIdAsync(cmd.OrganizationId, ct);
        if (org is null) return Result.NotFound<Organization>(cmd.OrganizationId);
        if (org.OwnerUserId != context.UserId) return Result.Failure("Faqat egasi a'zoni olib tashlashi mumkin.");

        org.RemoveMember(cmd.UserId);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
