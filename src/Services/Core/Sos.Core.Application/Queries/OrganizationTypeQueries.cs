using MediatR;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Queries;

public record OrganizationTypeDto(Guid Id, string Code, string NameUz, string NameRu, string? NameEn, string? Icon);

public record GetAllOrganizationTypesQuery : IRequest<Result<List<OrganizationTypeDto>>>;

public class GetAllOrganizationTypesHandler(IOrganizationTypeRepository repo)
    : IRequestHandler<GetAllOrganizationTypesQuery, Result<List<OrganizationTypeDto>>>
{
    public async Task<Result<List<OrganizationTypeDto>>> Handle(GetAllOrganizationTypesQuery _, CancellationToken ct)
    {
        var types = await repo.GetAllAsync(ct);
        return Result.Success(types.Select(t => new OrganizationTypeDto(t.Id, t.Code, t.NameUz, t.NameRu, t.NameEn, t.Icon)).ToList());
    }
}
