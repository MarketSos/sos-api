namespace Sos.Shared.Contracts.Events.Catalog;

public record ProductCreatedEvent(
    Guid Id, DateTime OccurredOn,
    Guid ProductId, string Name, string Barcode, decimal Price
) : IntegrationEvent(Id, OccurredOn);
