using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

public record ProductCreatedDomainEvent(Product Product) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
