using MediatR;
using Sos.Analytics.Application.Interfaces;
using Sos.Analytics.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Analytics.Application.Commands;

/// <summary>
/// POS servisidan sotuv ma'lumotini yozib olish.
/// </summary>
public record RecordSaleCommand(
    Guid    SaleId,
    Guid    StoreId,
    Guid    CashierId,
    decimal TotalAmount,
    int     ItemCount,
    DateTimeOffset CompletedAt,
    Guid?   CustomerId = null
) : IRequest<Result>;

public class RecordSaleHandler(ISaleSnapshotRepository repo)
    : IRequestHandler<RecordSaleCommand, Result>
{
    public async Task<Result> Handle(RecordSaleCommand cmd, CancellationToken ct)
    {
        var snapshot = SaleSnapshot.Create(
            cmd.SaleId, cmd.StoreId, cmd.CashierId,
            cmd.TotalAmount, cmd.ItemCount,
            cmd.CompletedAt, cmd.CustomerId);

        await repo.AddAsync(snapshot, ct);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
