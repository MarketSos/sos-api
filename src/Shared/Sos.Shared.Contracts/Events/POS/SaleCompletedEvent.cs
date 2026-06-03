namespace Sos.Shared.Contracts.Events.POS;

public record SaleCompletedEvent(
    Guid Id, DateTime OccurredOn,
    Guid SaleId, Guid StoreId, Guid? CustomerId,
    decimal TotalAmount, List<SaleItemDto> Items
) : IntegrationEvent(Id, OccurredOn);

public record SaleItemDto(Guid ProductId, int Quantity, decimal UnitPrice);
