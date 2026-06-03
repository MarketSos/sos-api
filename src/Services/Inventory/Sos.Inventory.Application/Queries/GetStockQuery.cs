using MediatR;
using Sos.Inventory.Application.Interfaces;
using Sos.Inventory.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Inventory.Application.Queries;

/// <summary>
/// Bitta mahsulotning ombordagi miqdorini olish.
/// </summary>
public record GetStockQuery(Guid ProductId, Guid StoreId) : IRequest<Result<StockItem>>;

public class GetStockHandler(IStockRepository repo) : IRequestHandler<GetStockQuery, Result<StockItem>>
{
    public async Task<Result<StockItem>> Handle(GetStockQuery q, CancellationToken ct)
    {
        var stock = await repo.GetAsync(q.ProductId, q.StoreId, ct);
        return stock is null
            ? Result.Failure<StockItem>("Omborda bu mahsulot topilmadi.")
            : Result.Success(stock);
    }
}

/// <summary>
/// Omborning minimal qoldiq ostida bo'lgan mahsulotlari ro'yxati.
/// </summary>
public record GetLowStockQuery(Guid StoreId) : IRequest<Result<IEnumerable<StockItem>>>;

public class GetLowStockHandler(IStockRepository repo) : IRequestHandler<GetLowStockQuery, Result<IEnumerable<StockItem>>>
{
    public async Task<Result<IEnumerable<StockItem>>> Handle(GetLowStockQuery q, CancellationToken ct)
    {
        var items = await repo.GetLowStockAsync(q.StoreId, ct);
        return Result.Success(items);
    }
}
