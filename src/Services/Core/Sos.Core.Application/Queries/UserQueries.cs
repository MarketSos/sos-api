using MediatR;
using Microsoft.AspNetCore.Identity;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities.Identity;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Queries;

// ── DTO ───────────────────────────────────────────────────────────────────────
public record UserDto(
    Guid     Id,
    string   UserName,
    string   Email,
    string[] Roles,
    bool     IsActive,
    Guid?    StoreId,
    Guid     OrganizationId);

// ── GetAllUsers ───────────────────────────────────────────────────────────────
public record GetAllUsersQuery : IRequest<Result<List<UserDto>>>;

public class GetAllUsersHandler(IUserRepository repo, UserManager<User> userManager)
    : IRequestHandler<GetAllUsersQuery, Result<List<UserDto>>>
{
    public async Task<Result<List<UserDto>>> Handle(GetAllUsersQuery _, CancellationToken ct)
    {
        var users  = await repo.GetAllAsync(ct);
        var result = new List<UserDto>(users.Count);

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(Mapper.ToDto(user, [.. roles]));
        }

        return Result.Success(result);
    }
}

// ── GetUserById ───────────────────────────────────────────────────────────────
public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;

public class GetUserByIdHandler(IUserRepository repo, UserManager<User> userManager)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery q, CancellationToken ct)
    {
        var user = await repo.GetByIdAsync(q.Id, ct);
        if (user is null) return Result.NotFound<UserDto, User>(q.Id);

        var roles = await userManager.GetRolesAsync(user);
        return Result.Success(Mapper.ToDto(user, [.. roles]));
    }
}

file static class Mapper
{
    internal static UserDto ToDto(User u, string[] roles) =>
        new(u.Id, u.UserName ?? "", u.Email ?? "", roles, u.IsActive, u.StoreId, u.OrganizationId);
}
