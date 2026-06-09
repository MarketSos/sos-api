using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Queries;

// ── DTO ───────────────────────────────────────────────────────────────────────
public record ManufacturerDto(
    Guid    Id,
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn,
    string? NameUzCyrl,
    string? NameKk,
    string? AddressLine,
    string? Phone
);

file static class ManufacturerMapper
{
    internal static ManufacturerDto ToDto(Manufacturer m) => new(
        m.Id, m.Code, m.NameUz, m.NameRu, m.NameEn, m.NameUzCyrl, m.NameKk, m.AddressLine, m.Phone);
}

// ── GetManufacturerById ───────────────────────────────────────────────────────
public record GetManufacturerByIdQuery(Guid Id) : IRequest<Result<ManufacturerDto>>;

public class GetManufacturerByIdHandler(IManufacturerRepository repo)
    : IRequestHandler<GetManufacturerByIdQuery, Result<ManufacturerDto>>
{
    public async Task<Result<ManufacturerDto>> Handle(GetManufacturerByIdQuery q, CancellationToken ct)
    {
        var m = await repo.GetByIdAsync(q.Id, ct);
        if (m is null) return Result.NotFound<ManufacturerDto, Manufacturer>(q.Id);
        return Result.Success(ManufacturerMapper.ToDto(m));
    }
}

// ── GetAllManufacturers ───────────────────────────────────────────────────────
public record GetAllManufacturersQuery : IRequest<Result<List<ManufacturerDto>>>;

public class GetAllManufacturersHandler(IManufacturerRepository repo)
    : IRequestHandler<GetAllManufacturersQuery, Result<List<ManufacturerDto>>>
{
    public async Task<Result<List<ManufacturerDto>>> Handle(GetAllManufacturersQuery _, CancellationToken ct)
    {
        var list = await repo.GetAllAsync(ct);
        return Result.Success(list.Select(ManufacturerMapper.ToDto).ToList());
    }
}
