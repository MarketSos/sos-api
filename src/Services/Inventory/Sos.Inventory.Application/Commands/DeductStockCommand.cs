using MediatR;
using Sos.Inventory.Application.Interfaces;
using Sos.Shared.Kernel.Results;

namespace Sos.Inventory.Application.Commands;

public record DeductStockCommand(Guid ProductId, Guid StoreId, int Amount) : IRequest<Result>;

public class DeductStockHandler(IStockRepository repo) : IRequestHandler<DeductStockCommand, Result>
{
    public async Task<Result> Handle(DeductStockCommand cmd, CancellationToken ct)
    {
        var stock = await repo.GetAsync(cmd.ProductId, cmd.StoreId, ct);
        if (stock is null) return Result.Failure("Stock record not found.");

        var r = stock.Deduct(cmd.Amount);
        if (!r.IsSuccess) return Result.Failure(r.Error!);

        await repo.UpdateAsync(stock, ct);
        return Result.