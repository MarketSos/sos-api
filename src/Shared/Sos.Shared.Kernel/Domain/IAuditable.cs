namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Аудит записи — кто и когда создал/изменил.
/// </summary>
public interface IAuditable
{
    /// <summary>Дата создания (UTC)</summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>Дата последнего изменения (UTC)</summary>
    DateTimeOffset? UpdatedAt { get; }

    /// <summary>ID пользователя, создавшего запись</summary>
    Guid? CreatedBy { get; }

    /// <summary>ID пользователя, последним изменившего запись</summary>
    Guid? UpdatedBy { get; }
}
