namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Базовый класс для сущностей с многоязычным названием и флагом активности.
/// </summary>
public abstract class ActiveEntity<TId> : LocalizableEntity<TId>
{
    /// <summary>
    /// Активна ли сущность
    /// </summary>
    public bool IsActive { get; protected set; } = true;

    /// <summary>
    /// Деактивировать сущность
    /// </summary>
    public virtual void Deactivate() => IsActive = false;

    /// <summary>
    /// Активировать сущность
    /// </summary>
    public virtual void Activate() => IsActive = true;
}
