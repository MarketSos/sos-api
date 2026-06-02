namespace Sos.Shared.Kernel.Domain;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    public int Version { get; protected set; }
}
