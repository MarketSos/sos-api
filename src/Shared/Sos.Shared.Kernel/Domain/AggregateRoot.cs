namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Корень агрегата — точка входа для изменений внутри агрегата.
/// Версия используется для оптимистичной конкуренции.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>
{
    /// <summary>
    /// Версия агрегата для оптимистичной блокировки
    /// </summary>
    public int Version { get; protected set; }
}
