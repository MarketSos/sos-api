namespace Sos.Shared.Contracts.Events.POS;

/// <summary>
/// Sotuv yakunlanganda chiqariladigan hodisa.
/// Событие завершения продажи.
/// </summary>
public record SaleCompletedEvent(
    Guid             Id,
    DateTime         OccurredOn,
    Guid             SaleId,
    Guid             StoreId,
    Guid             CashierId,
    Guid?            CustomerId,
    decimal          TotalAmount,
    List<SaleItemDto> Items
) : IntegrationEvent(Id, OccurredOn);

public record SaleItemDto(Guid ProductId, int Quantity, decimal UnitPrice);
