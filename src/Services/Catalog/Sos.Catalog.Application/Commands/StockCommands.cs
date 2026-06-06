using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Commands;

// ── AddStock ──────────────────────────────────────────────────────────────────
public record AddStockCommand(Guid ProductId, Guid StoreId, int Amount, int MinQuantity = 0)
    : IRequest<Result>;

public class AddStockHandler(IStockRepository repo) : IRequestHandler<AddStockCommand, Result>
{
    public async Task<Result> Handle(AddStockCommand cmd, CancellationToken ct)
    {
        if (cmd.Amount <= 0)
            return Result.Failure("Miqdor 0 dan katta bo'lishi kerak.");

        var stock = await repo.GetAsync(cmd.ProductId, cmd.StoreId, ct);

        if (stock is null)
        {
            stock = StockItem.Create(cmd.ProductId, cmd.StoreId, cmd.Amount, cmd.MinQuantity);
            await repo.AddAsync(stock, ct);
        }
        else
        {
            stock.Add(cmd.Amount);
        }

        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── DeductStock ───────────────────────────────────────────────────────────────
public record DeductStockCommand(Guid ProductId, Guid StoreId, int Amount) : IRequest<Result>;

public class DeductStockHandler(IStockRepository repo) : IRequestHandler<DeductStockCommand, Result>
{
    public async Task<Result> Handle(DeductStockCommand cmd, CancellationToken ct)
    {
        var stock = await repo.GetAsync(cmd.ProductId, cmd.StoreId, ct);
        if (stock is null) return Result.Failure("Stock topilmadi.");

        var result = stock.Deduct(cmd.Amount);
        if (!result.IsSuccess) return Result.Failure(result.Error!);

        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
