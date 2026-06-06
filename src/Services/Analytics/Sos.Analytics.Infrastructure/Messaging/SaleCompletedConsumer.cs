using MassTransit;
using Microsoft.Extensions.Logging;
using Sos.Analytics.Application.Interfaces;
using Sos.Analytics.Domain.Entities;
using Sos.Shared.Contracts.Events.POS;

namespace Sos.Analytics.Infrastructure.Messaging;

/// <summary>
/// SaleCompleted hodisasini qabul qilib, analytics uchun snapshot saqlaydi.
/// Получает SaleCompleted и сохраняет снапшот для аналитики.
/// </summary>
public class SaleCompletedConsumer(
    ISaleSnapshotRepository repo,
    ILogger<SaleCompletedConsumer> logger)
    : IConsumer<SaleCompletedEvent>
{
    public async Task Consume(ConsumeContext<SaleCompletedEvent> context)
    {
        var e = context.Message;
        logger.LogInformation(
            "Analytics: SaleCompleted qabul qilindi. SaleId: {SaleId}, Summa: {Amount}",
            e.SaleId, e.TotalAmount);

        var snapshot = SaleSnapshot.Create(
            saleId:      e.SaleId,
            storeId:     e.StoreId,
            cashierId:   e.CashierId,
            totalAmount: e.TotalAmount,
            itemCount:   e.Items.Count,
            completedAt: e.OccurredOn,
            customerId:  e.CustomerId);

        await repo.AddAsync(snapshot, context.CancellationToken);
        await repo.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("Analytics snapshot saqlandi. SaleId: {SaleId}", e.SaleId);
    }
}
