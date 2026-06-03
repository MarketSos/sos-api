namespace Sos.Shared.Contracts.Events.Inventory;

public record StockUpdatedEvent(
    Guid Id, DateTime OccurredOn,
    Guid ProductId, Guid StoreId, int NewQuantity
) : IntegrationEvent(Id, Occurred