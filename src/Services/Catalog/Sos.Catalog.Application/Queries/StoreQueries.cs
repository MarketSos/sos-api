using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Queries;

// ── DTO ───────────────────────────────────────────────────────────────────────
public record StoreDto(
    Guid    Id,
    Guid    OrganizationId,
    string  Code,
    string  Name,
    string? Address,
    string? Phone,
    bool    IsActive
);

file static class StoreMapper
{
    internal static StoreDto ToDto(Store s) => new(
        s.Id, s.OrganizationId, s.Code, s.Name, s.Address, s.Phone, s.IsActive);
}

// ── GetStoreById ──────────────────────────────────────────────────────────────
public record GetStoreByIdQuery(Guid Id) : IRequest<Result<StoreDto>>;

public class GetStoreByIdHandler(IStoreRepository repo)
    : IRequestHandler<GetStoreByIdQuery, Result<StoreDto>>
{
    public async Task<Result<StoreDto>> Handle(GetStoreByIdQuery q, CancellationToken ct)
    {
        var store = await repo.GetByIdAsync(q.Id, ct);
        if (store is null) return Result.NotFound<StoreDto, Store>(q.Id);
        return Result.Success(StoreMapper.ToDto(store));
    }
}

// ── GetStoresByOrganization ───────────────────────────────────────────────────
public record GetStoresByOrganizationQuery(Guid OrganizationId) : IRequest<Result<List<StoreDto>>>;

public class GetStoresByOrganizationHandler(IStoreRepository repo)
    : IRequestHandler<GetStoresByOrganizationQuery, Result<List<StoreDto>>>
{
    public async Task<Result<List<StoreDto>>> Handle(GetStoresByOrganizationQuery q, CancellationToken ct)
    {
        var list = await repo.GetByOrganizationAsync(q.OrganizationId, ct);
        return Result.Success(list.Select(StoreMapper.ToDto).ToList());
    }
}
