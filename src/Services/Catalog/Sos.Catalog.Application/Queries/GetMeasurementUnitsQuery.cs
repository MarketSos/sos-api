using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Queries;

public record GetMeasurementUnitsQuery : IRequest<Result<IEnumerable<MeasurementUnit>>>;

public class GetMeasurementUnitsHandler(IMeasurementUnitRepository repo)
    : IRequestHandler<GetMeasurementUnitsQuery, Result<IEnumerable<MeasurementUnit>>>
{
    public async Task<Result<IEnumerable<MeasurementUnit>>> Handle(GetMeasurementUnitsQuery q, CancellationToken ct)
    {
        var units = await repo.GetAllAsync(ct);
        return Result.Success(units);
    }
}
