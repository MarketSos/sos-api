using MediatR;
using Sos.Core.Application.Constants;
using Sos.Core.Domain.Entities.Identity;
using Sos.Shared.Kernel.Results;
using Microsoft.AspNetCore.Identity;

namespace Sos.Core.Application.Queries;

public record RoleDto(
    Guid                  Id,
    string                Name,
    string                NameUz,
    string                NameRu,
    string?               NameEn,
    string?               NameKa,
    IReadOnlyList<string> Permissions);

public record RoleSummaryDto(
    Guid    Id,
    string  Name,
    string  NameUz,
    string  NameRu,
    string? NameEn,
    string? NameKa,
    int     PermissionCount);

public record GetAllRolesQuery : IRequest<Result<List<RoleSummaryDto>>>;

public class GetAllRolesHandler(RoleManager<Role> roleManager)
    : IRequestHandler<GetAllRolesQuery, Result<List<RoleSummaryDto>>>
{
    public async Task<Result<List<RoleSummaryDto>>> Handle(GetAllRolesQuery _, CancellationToken ct)
    {
        var result = new List<RoleSummaryDto>();
        foreach (var role in roleManager.Roles.OrderBy(r => r.Name).ToList())
        {
            var claims = await roleManager.GetClaimsAsync(role);
            var count  = claims.Count(c => c.Type == ClaimsType.Permission);
            result.Add(new RoleSummaryDto(role.Id, role.Name!, role.NameUz, role.NameRu, role.NameEn, role.NameKa, count));
        }
        return Result.Success(result);
    }
}

public record GetRoleByNameQuery(string Name) : IRequest<Result<RoleDto>>;

public class GetRoleByNameHandler(RoleManager<Role> roleManager)
    : IRequestHandler<GetRoleByNameQuery, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(GetRoleByNameQuery q, CancellationToken ct)
    {
        var role = await roleManager.FindByNameAsync(q.Name);
        if (role is null) return Result.NotFound<RoleDto, Role>(q.Name);

        var claims = await roleManager.GetClaimsAsync(role);
        var permissions = claims
            .Where(c => c.Type == ClaimsType.Permission)
            .Select(c => c.Value)
            .OrderBy(p => p)
            .ToList();

        return Result.Success(new RoleDto(role.Id, role.Name!, role.NameUz, role.NameRu, role.NameEn, role.NameKa, permissions));
    }
}
