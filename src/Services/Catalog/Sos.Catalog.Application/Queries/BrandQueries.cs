using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Queries;

// ── DTO ───────────────────────────────────────────────────────────────────────
public record BrandDto(
    Guid    Id,
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn,
    string? NameUzKiril
);

file static class BrandMapper
{
    internal static BrandDto ToDto(Brand b) => new(
        b.Id, b.Code, b.NameUz, b.NameRu, b.NameEn, b.NameUzKiril);
}

// ── GetBrandById ──────────────────────────────────────────────────────────────
public record GetBrandByIdQuery(Guid Id) : IRequest<Result<BrandDto>>;

public class GetBrandByIdHandler(IBrandRepository repo)
    : IRequestHandler<GetBrandByIdQuery, Result<BrandDto>>
{
    public async Task<Result<BrandDto>> Handle(GetBrandByIdQuery q, CancellationToken ct)
    {
        var b = await repo.GetByIdAsync(q.Id, ct);
        if (b is null) return Result.NotFound<BrandDto, Brand>(q.Id);
        return Result.Success(BrandMapper.ToDto(b));
    }
}

// ── GetAllBrands ──────────────────────────────────────────────────────────────
public record GetAllBrandsQuery : IRequest<Result<List<BrandDto>>>;

public class GetAllBrandsHandler(IBrandRepository repo)
    : IRequestHandler<GetAllBrandsQuery, Result<List<BrandDto>>>
{
    public async Task<Result<List<BrandDto>>> Handle(GetAllBrandsQuery _, CancellationToken ct)
    {
        var list = await repo.GetAllAsync(ct);
        return Result.Success(list.Select(BrandMapper.ToDto).ToList());
    }
}
