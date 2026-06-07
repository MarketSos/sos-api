using MediatR;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Commands;

public record CreateOrganizationTypeCommand(string Code, string NameUz, string NameRu, string? NameEn = null, string? NameUzKiril = null, string? Icon = null)
    : IRequest<Result<Guid>>;

public class CreateOrganizationTypeHandler(IOrganizationTypeRepository repo)
    : IRequestHandler<CreateOrganizationTypeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrganizationTypeCommand cmd, CancellationToken ct)
    {
        var orgType = OrganizationType.Create(Guid.NewGuid(), cmd.Code, cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.NameUzKiril, cmd.Icon);
        await repo.AddAsync(orgType, ct);
        return Result.Success(orgType.Id);
    }
}

public record UpdateOrganizationTypeCommand(Guid Id, string NameUz, string NameRu, string? NameEn = null, string? NameUzKiril = null, string? Icon = null)
    : IRequest<Result>;

public class UpdateOrganizationTypeHandler(IOrganizationTypeRepository repo)
    : IRequestHandler<UpdateOrganizationTypeCommand, Result>
{
    public async Task<Result> Handle(UpdateOrganizationTypeCommand cmd, CancellationToken ct)
    {
        var orgType = await repo.GetByIdAsync(cmd.Id, ct);
        if (orgType is null) return Result.NotFound<OrganizationType>(cmd.Id);
        orgType.Update(cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.NameUzKiril, cmd.Icon);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public record DeleteOrganizationTypeCommand(Guid Id) : IRequest<Result>;

public class DeleteOrganizationTypeHandler(IOrganizationTypeRepository repo)
    : IRequestHandler<DeleteOrganizationTypeCommand, Result>
{
    public async Task<Result> Handle(DeleteOrganizationTypeCommand cmd, CancellationToken ct)
    {
        var orgType = await repo.GetByIdAsync(cmd.Id, ct);
        if (orgType is null) return Result.NotFound<OrganizationType>(cmd.Id);
        await repo.DeleteAsync(orgType, ct);
        return Result.Success();
    }
}
