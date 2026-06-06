using MediatR;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Domain.Enums;
using Sos.Shared.Infrastructure.Services;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Commands;

// ── CreateOrganization ────────────────────────────────────────────────────────
public record CreateOrganizationCommand(
    string            NameUz,
    string            NameRu,
    string            Slug,
    string?           NameEn      = null,
    string?           NameUzKiril = null,
    string?           Code        = null,
    string?           Tin         = null,
    string?           Okonx       = null,
    string?           Oked        = null,
    OrganizationType? OrgType     = null
) : IRequest<Result<Guid>>;

public class CreateOrganizationHandler(IOrganizationRepository repo, ICurrentContext context)
    : IRequestHandler<CreateOrganizationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrganizationCommand cmd, CancellationToken ct)
    {
        if (context.UserId is null)
            return Result.Failure<Guid>("Foydalanuvchi autentifikatsiyadan o'tmagan.");

        if (await repo.SlugExistsAsync(cmd.Slug, ct))
            return Result.Failure<Guid>($"'{cmd.Slug}' slug allaqachon band.");

        if (cmd.Code is not null && await repo.CodeExistsAsync(cmd.Code, ct))
            return Result.Failure<Guid>($"'{cmd.Code}' kod allaqachon mavjud.");

        var org = Organization.Create(
            Guid.NewGuid(), cmd.NameUz, cmd.NameRu, cmd.Slug,
            context.UserId.Value, cmd.NameEn, cmd.NameUzKiril);

        org.Code    = cmd.Code;
        org.Tin     = cmd.Tin;
        org.Okonx   = cmd.Okonx;
        org.Oked    = cmd.Oked;
        org.OrgType = cmd.OrgType;

        await repo.AddAsync(org, ct);
        return Result.Success(org.Id);
    }
}

// ── UpdateOrganizationNames ───────────────────────────────────────────────────
public record UpdateOrganizationNamesCommand(
    Guid    OrganizationId,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzKiril = null
) : IRequest<Result>;

public class UpdateOrganizationNamesHandler(IOrganizationRepository repo, ICurrentContext context)
    : IRequestHandler<UpdateOrganizationNamesCommand, Result>
{
    public async Task<Result> Handle(UpdateOrganizationNamesCommand cmd, CancellationToken ct)
    {
        var org = await repo.GetByIdAsync(cmd.OrganizationId, ct);
        if (org is null) return Result.Failure("Tashkilot topilmadi.");
        if (org.OwnerUserId != context.UserId) return Result.Failure("Faqat egasi o'zgartirishi mumkin.");

        org.UpdateNames(cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.NameUzKiril);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── UpdateOrganization ────────────────────────────────────────────────────────
public record UpdateOrganizationCommand(
    Guid              OrganizationId,
    string?           Code    = null,
    string?           Tin     = null,
    string?           Okonx   = null,
    string?           Oked    = null,
    OrganizationType? OrgType = null,
    bool              IsTest  = false
) : IRequest<Result>;

public class UpdateOrganizationHandler(IOrganizationRepository repo, ICurrentContext context)
    : IRequestHandler<UpdateOrganizationCommand, Result>
{
    public async Task<Result> Handle(UpdateOrganizationCommand cmd, CancellationToken ct)
    {
        var org = await repo.GetByIdAsync(cmd.OrganizationId, ct);
        if (org is null) return Result.Failure("Tashkilot topilmadi.");
        if (org.OwnerUserId != context.UserId) return Result.Failure("Faqat egasi o'zgartirishi mumkin.");

        org.Code    = cmd.Code;
        org.Tin     = cmd.Tin;
        org.Okonx   = cmd.Okonx;
        org.Oked    = cmd.Oked;
        org.OrgType = cmd.OrgType;
        org.IsTest  = cmd.IsTest;

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
        if (org is null) return Result.Failure("Tashkilot topilmadi.");
        if (org.OwnerUserId != context.UserId) return Result.Failure("Faqat egasi o'zgartirishi mumkin.");

        if (cmd.ParentId.HasValue)
        {
            var parent = await repo.GetByIdAsync(cmd.ParentId.Value, ct);
            if (parent is null) return Result.Failure("Yuqori tashkilot topilmadi.");
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
        if (org is null) return Result.Failure("Tashkilot topilmadi.");
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
        if (org is null) return Result.Failure("Tashkilot topilmadi.");
        if (org.OwnerUserId != context.UserId) return Result.Failure("Faqat egasi o'chirishi mumkin.");

        org.SoftDelete(context.UserId);
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
        if (org is null) return Result.Failure<Guid>("Tashkilot topilmadi.");
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
        if (org is null) return Result.Failure("Tashkilot topilmadi.");
        if (org.OwnerUserId != context.UserId) return Result.Failure("Faqat egasi a'zoni olib tashlashi mumkin.");

        org.RemoveMember(cmd.UserId);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
