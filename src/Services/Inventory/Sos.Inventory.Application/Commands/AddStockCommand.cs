using MediatR;
using Sos.Inventory.Application.Interfaces;
using Sos.Inventory.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Inventory.Application.Commands;

/// <summary>
/// Tovar omborga kiritish (qabul qilish yoki sozlash).
/// Agar stock mavjud bo'lmasa — yangi yozuv yaratiladi.
/// </summary>
public record AddStockCommand(
    Guid ProductId,
    Guid StoreId,
    int Amount,
    int MinQuantity = 0
) : IRequest<Result>;

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
            await repo.UpdateAsync(stock, ct);
        }

        return Result.Success();
    }
}
