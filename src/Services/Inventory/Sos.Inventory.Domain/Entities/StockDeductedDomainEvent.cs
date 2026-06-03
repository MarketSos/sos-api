using Sos.Shared.Kernel.Domain;

namespace Sos.Inventory.Domain.Entities;

public record StockDeductedDomainEvent(StockItem StockItem) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
