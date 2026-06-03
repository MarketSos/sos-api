namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Интерфейс аудита — кто и когда создал/изменил запись.
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// ID пользователя, создавшего запись
    /// </summary>
    public Guid? CreatedBy { get; }

    /// <summary>
    /// ID пользователя, последним изменившего запись
    /// </summary>
    public Guid? UpdatedBy { get; }
}
