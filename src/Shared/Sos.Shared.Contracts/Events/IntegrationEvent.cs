namespace Sos.Shared.Contracts.Events;

public abstract record IntegrationEvent(Guid Id, DateTime OccurredOn);
