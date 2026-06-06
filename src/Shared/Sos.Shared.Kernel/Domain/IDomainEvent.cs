using MediatR;

namespace Sos.Shared.Kernel.Domain;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTimeOffset OccurredOn { get; }
}
