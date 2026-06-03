using Sos.Shared.Kernel.Domain;

namespace Sos.POS.Domain.Entities;

public record SaleCompletedDomainEvent(Sale Sale) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
