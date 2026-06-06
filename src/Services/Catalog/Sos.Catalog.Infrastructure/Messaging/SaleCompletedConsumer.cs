using MassTransit;
using Microsoft.Extensions.Logging;
using Sos.Catalog.Application.Interfaces;
using Sos.Shared.Contracts.Events.POS;

namespace Sos.Catalog.Infrastructure.Messaging;

/// <summary>
/// SaleCompleted hodisasini qabul qilib, har bir tovar uchun stock ni kamaytiradi.
/// Получает SaleCompleted и списывает остатки по каждому товару.
/// </summary>
public class SaleCompletedConsumer(
    IStockRepository  stockRepo,
    ILogger<SaleCompletedConsumer> logger)
    : IConsumer<SaleCompletedEvent>
{
    public async Task Consume(ConsumeContext<SaleCompletedEvent> context)
    {
        var e = context.Message;
        logger.LogInformation(
            "SaleCompleted qabul qilindi. SaleId: {SaleId}, Do'kon: {StoreId}, Tovarlar: {Count}",
            e.SaleId, e.StoreId, e.Items.Count);

        foreach (var item in e.Items)
        {
            var stock = await stockRepo.GetAsync(item.ProductId, e.StoreId, context.CancellationToken);

            if (stock is null)
            {
                logger.LogWarning(
                    "Stock topilmadi. ProductId: {ProductId}, StoreId: {StoreId}",
                    item.ProductId, e.StoreId);
                continue;
            }

            var result = stock.Deduct(item.Quantity);

            if (!result.IsSuccess)
            {
                logger.LogWarning(
                    "Stock kamaytirishda xato. ProductId: {ProductId}, Xato: {Error}",
                    item.ProductId, result.Error);
                continue;
            }

            await stockRepo.SaveChangesAsync(context.CancellationToken);

            logger.LogInformation(
                "Stock kamaytirildi. ProductId: {ProductId}, Kamaydi: {Qty}, Qoldi: {Left}",
                item.ProductId, item.Quantity, stock.Quantity);
        }
    }
}
