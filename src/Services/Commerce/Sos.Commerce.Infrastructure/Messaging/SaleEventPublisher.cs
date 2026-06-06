using MassTransit;
using Sos.Commerce.Application.Interfaces;
using Sos.Commerce.Domain.Entities;
using Sos.Shared.Contracts.Events.POS;

namespace Sos.Commerce.Infrastructure.Messaging;

/// <summary>
/// SaleCompleted hodisasini RabbitMQ ga yuboruvchi.
/// Публикатор события SaleCompleted в RabbitMQ.
/// </summary>
public class SaleEventPublisher(IPublishEndpoint publish) : ISaleEventPublisher
{
    public Task PublishSaleCompletedAsync(Sale sale, CancellationToken ct = default)
    {
        var @event = new SaleCompletedEvent(
            Id:          Guid.NewGuid(),
            OccurredOn:  DateTime.UtcNow,
            SaleId:      sale.Id,
            StoreId:     sale.StoreId,
            CashierId:   sale.CashierId,
            CustomerId:  sale.CustomerId,
            TotalAmount: sale.TotalAmount,
            Items:       sale.Items
                             .Select(i => new SaleItemDto(i.ProductId, i.Quantity, i.UnitPrice))
                             .ToList());

        return publish.Publish(@event, ct);
    }
}
