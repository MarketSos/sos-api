using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Queries;

public record StockItemDto(
    Guid    Id,
    Guid    ProductId,
    string  ProductNameUz,
    string  ProductNameRu,
    string  Barcode,
    Guid    StoreId,
    int     Quantity,
    int     MinQuantity,
    int?    MaxQuantity,
    string? Location,
    bool    IsLow
);

public record GetStockListQuery(Guid? StoreId = null) : IRequest<Result<IEnumerable<StockItemDto>>>;

public class GetStockListHandler(IStockRepository stockRepo, IProductRepository productRepo)
    : IRequestHandler<GetStockListQuery, Result<IEnumerable<StockItemDto>>>
{
    public async Task<Result<IEnumerable<StockItemDto>>> Handle(GetStockListQuery q, CancellationToken ct)
    {
        var stocks = await stockRepo.GetByStoreAsync(q.StoreId, ct);

        var productIds = stocks.Select(s => s.ProductId).Distinct().ToList();
        var products   = new Dictionary<Guid, (string NameUz, string NameRu, string Barcode)>();

        foreach (var id in productIds)
        {
            var p = await productRepo.GetByIdAsync(id, ct);
            if (p is not null)
                products[id] = (p.NameUz, p.NameRu, p.Barcode);
        }

        var dtos = stocks.Select(s =>
        {
            products.TryGetValue(s.ProductId, out var p);
            return new StockItemDto(
                s.Id,
                s.ProductId,
                p.NameUz ?? "—",
                p.NameRu ?? "—",
                p.Barcode ?? "—",
                s.StoreId,
                s.Quantity,
                s.MinQuantity,
                s.MaxQuantity,
                s.Location,
                s.IsLow);
        });

        return Result.Success(dtos);
    }
}
