using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Sos.Core.Application.Constants;
using Sos.Core.Domain.Entities.Identity;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Commands;

public record CreateRoleCommand(
    string  Name,
    string  NameUz,
    string  NameUzCyrl,
    string  NameRu,
    string? NameEn,
    string? NameKk) : IRequest<Result<Guid>>;

public class CreateRoleHandler(RoleManager<Role> roleManager)
    : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRoleCommand cmd, CancellationToken ct)
    {
        if (await roleManager.RoleExistsAsync(cmd.Name))
            return Result.Conflict<Guid>("Role", cmd.Name);

        var role = new Role
        {
            Name      = cmd.Name,
            NameUz    = cmd.NameUz,
            NameUzCyrl = cmd.NameUzCyrl,
            NameRu    = cmd.NameRu,
            NameEn    = cmd.NameEn,
            NameKk    = cmd.NameKk,
        };
        var result = await roleManager.CreateAsync(role);
        return result.Succeeded
            ? Result.Success(role.Id)
            : Result.Failure<Guid>(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}

public record UpdateRoleCommand(
    string  Name,
    string  NameUz,
    string  NameUzCyrl,
    string  NameRu,
    string? NameEn,
    string? NameKk) : IRequest<Result>;

public class UpdateRoleHandler(RoleManager<Role> roleManager)
    : IRequestHandler<UpdateRoleCommand, Result>
{
    public async Task<Result> Handle(UpdateRoleCommand cmd, CancellationToken ct)
    {
        var role = await roleManager.FindByNameAsync(cmd.Name);
        if (role is null) return Result.NotFound("Role", cmd.Name);

        role.NameUz    = cmd.NameUz;
        role.NameUzCyrl = cmd.NameUzCyrl;
        role.NameRu    = cmd.NameRu;
        role.NameEn    = cmd.NameEn;
        role.NameKk    = cmd.NameKk;
        var result = await roleManager.UpdateAsync(role);
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}

public record DeleteRoleCommand(string Name) : IRequest<Result>;

public class DeleteRoleHandler(RoleManager<Role> roleManager)
    : IRequestHandler<DeleteRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteRoleCommand cmd, CancellationToken ct)
    {
        var role = await roleManager.FindByNameAsync(cmd.Name);
        if (role is null) return Result.NotFound("Role", cmd.Name);

        var result = await roleManager.DeleteAsync(role);
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}

public record AddRolePermissionCommand(string RoleName, string Permission) : IRequest<Result>;

public class AddRolePermissionHandler(RoleManager<Role> roleManager)
    : IRequestHandler<AddRolePermissionCommand, Result>
{
    public async Task<Result> Handle(AddRolePermissionCommand cmd, CancellationToken ct)
    {
        var role = await roleManager.FindByNameAsync(cmd.RoleName);
        if (role is null) return Result.NotFound("Role", cmd.RoleName);

        var existing = await roleManager.GetClaimsAsync(role);
        if (existing.Any(c => c.Type == ClaimsType.Permission && c.Value == cmd.Permission))
            return Result.Conflict("Permission", cmd.Permission);

        var result = await roleManager.AddClaimAsync(role, new Claim(ClaimsType.Permission, cmd.Permission));
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}

public record RemoveRolePermissionCommand(string RoleName, string Permission) : IRequest<Result>;

public class RemoveRolePermissionHandler(RoleManager<Role> roleManager)
    : IRequestHandler<RemoveRolePermissionCommand, Result>
{
    public async Task<Result> Handle(RemoveRolePermissionCommand cmd, CancellationToken ct)
    {
        var role = await roleManager.FindByNameAsync(cmd.RoleName);
        if (role is null) return Result.NotFound<Role>("Role", cmd.RoleName);

        var existing = await roleManager.GetClaimsAsync(role);
        if (!existing.Any(c => c.Type == ClaimsType.Permission && c.Value == cmd.Permission))
            return Result.Failure($"Role [{cmd.RoleName}] da '{cmd.Permission}' permission mavjud emas.");

        var result = await roleManager.RemoveClaimAsync(role, new Claim(ClaimsType.Permission, cmd.Permission));
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
