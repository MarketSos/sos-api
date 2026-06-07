using MediatR;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Commands;

public record CreateOrgTypeCommand(string Code, string NameUz, string NameRu, string? NameEn = null, string? Icon = null)
    : IRequest<Result<Guid>>;

public class CreateOrgTypeHandler(IOrgTypeRepository repo)
    : IRequestHandler<CreateOrgTypeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrgTypeCommand cmd, CancellationToken ct)
    {
        var orgType = OrgType.Create(Guid.NewGuid(), cmd.Code, cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.Icon);
        await repo.AddAsync(orgType, ct);
        return Result.Success(orgType.Id);
    }
}

public record UpdateOrgTypeCommand(Guid Id, string NameUz, string NameRu, string? NameEn = null, string? Icon = null)
    : IRequest<Result>;

public class UpdateOrgTypeHandler(IOrgTypeRepository repo)
    : IRequestHandler<UpdateOrgTypeCommand, Result>
{
    public async Task<Result> Handle(UpdateOrgTypeCommand cmd, CancellationToken ct)
    {
        var orgType = await repo.GetByIdAsync(cmd.Id, ct);
        if (orgType is null) return Result.NotFound<OrgType>(cmd.Id);
        orgType.Update(cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.Icon);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public record DeleteOrgTypeCommand(Guid Id) : IRequest<Result>;

public class DeleteOrgTypeHandler(IOrgTypeRepository repo)
    : IRequestHandler<DeleteOrgTypeCommand, Result>
{
    public async Task<Result> Handle(DeleteOrgTypeCommand cmd, CancellationToken ct)
    {
        var orgType = await repo.GetByIdAsync(cmd.Id, ct);
        if (orgType is null) return Result.NotFound<OrgType>(cmd.Id);
        await repo.DeleteAsync(orgType, ct);
        return Result.Success();
    }
}
