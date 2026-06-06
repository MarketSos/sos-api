using MediatR;
using Microsoft.AspNetCore.Identity;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities.Identity;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Commands;

// ── ChangeUserRole ────────────────────────────────────────────────────────────
public record ChangeUserRoleCommand(Guid UserId, Guid[] RoleIds) : IRequest<Result>;

public class ChangeUserRoleHandler(
    IUserRepository                 repo,
    UserManager<User>               userManager,
    RoleManager<Role> roleManager)
    : IRequestHandler<ChangeUserRoleCommand, Result>
{
    public async Task<Result> Handle(ChangeUserRoleCommand cmd, CancellationToken ct)
    {
        var user = await repo.GetByIdAsync(cmd.UserId, ct);
        if (user is null) return Result.NotFound<User>(cmd.UserId);

        var oldRoles = await userManager.GetRolesAsync(user);
        if (oldRoles.Any())
            await userManager.RemoveFromRolesAsync(user, oldRoles);

        foreach (var roleId in cmd.RoleIds)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            if (role?.Name is not null)
                await userManager.AddToRoleAsync(user, role.Name);
        }

        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── AssignUserToStore ─────────────────────────────────────────────────────────
public record AssignUserToStoreCommand(Guid UserId, Guid? StoreId) : IRequest<Result>;

public class AssignUserToStoreHandler(IUserRepository repo)
    : IRequestHandler<AssignUserToStoreCommand, Result>
{
    public async Task<Result> Handle(AssignUserToStoreCommand cmd, CancellationToken ct)
    {
        var user = await repo.GetByIdAsync(cmd.UserId, ct);
        if (user is null) return Result.NotFound<User>(cmd.UserId);

        user.SetStore(cmd.StoreId);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── ChangeUserPassword ────────────────────────────────────────────────────────
public record ChangeUserPasswordCommand(
    Guid   UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<Result>;

public class ChangeUserPasswordHandler(
    IUserRepository   repo,
    UserManager<User> userManager)
    : IRequestHandler<ChangeUserPasswordCommand, Result>
{
    public async Task<Result> Handle(ChangeUserPasswordCommand cmd, CancellationToken ct)
    {
        var user = await repo.GetByIdAsync(cmd.UserId, ct);
        if (user is null) return Result.NotFound<User>(cmd.UserId);

        var result = await userManager.ChangePasswordAsync(user, cmd.CurrentPassword, cmd.NewPassword);
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}

// ── ResetUserPassword (SuperAdmin) ────────────────────────────────────────────
public record ResetUserPasswordCommand(Guid UserId, string NewPassword) : IRequest<Result>;

public class ResetUserPasswordHandler(
    IUserRepository   repo,
    UserManager<User> userManager)
    : IRequestHandler<ResetUserPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetUserPasswordCommand cmd, CancellationToken ct)
    {
        var user = await repo.GetByIdAsync(cmd.UserId, ct);
        if (user is null) return Result.NotFound<User>(cmd.UserId);

        var token  = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, cmd.NewPassword);
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}

// ── ActivateUser ──────────────────────────────────────────────────────────────
public record ActivateUserCommand(Guid UserId) : IRequest<Result>;

public class ActivateUserHandler(IUserRepository repo)
    : IRequestHandler<ActivateUserCommand, Result>
{
    public async Task<Result> Handle(ActivateUserCommand cmd, CancellationToken ct)
    {
        var user = await repo.GetByIdAsync(cmd.UserId, ct);
        if (user is null) return Result.NotFound<User>(cmd.UserId);
        if (user.IsActive) return Result.Failure("Foydalanuvchi allaqachon faol.");

        user.Activate();
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── DeactivateUser ────────────────────────────────────────────────────────────
public record DeactivateUserCommand(Guid UserId) : IRequest<Result>;

public class DeactivateUserHandler(IUserRepository repo)
    : IRequestHandler<DeactivateUserCommand, Result>
{
    public async Task<Result> Handle(DeactivateUserCommand cmd, CancellationToken ct)
    {
        var user = await repo.GetByIdAsync(cmd.UserId, ct);
        if (user is null) return Result.NotFound<User>(cmd.UserId);
        if (!user.IsActive) return Result.Failure("Foydalanuvchi allaqachon bloklangan.");

        user.Deactivate();
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
