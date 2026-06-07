using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Queries;

public record OrgTypeDto(Guid Id, string Code, string NameUz, string NameRu, string? NameEn, string? Icon);

public record GetAllOrgTypesQuery : IRequest<Result<List<OrgTypeDto>>>;

public class GetAllOrgTypesHandler(IOrgTypeRepository repo)
    : IRequestHandler<GetAllOrgTypesQuery, Result<List<OrgTypeDto>>>
{
    public async Task<Result<List<OrgTypeDto>>> Handle(GetAllOrgTypesQuery _, CancellationToken ct)
    {
        var types = await repo.GetAllAsync(ct);
        return Result.Success(types.Select(t => new OrgTypeDto(t.Id, t.Code, t.NameUz, t.NameRu, t.NameEn, t.Icon)).ToList());
    }
}
