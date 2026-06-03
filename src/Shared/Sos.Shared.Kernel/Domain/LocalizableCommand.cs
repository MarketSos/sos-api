namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Базовая запись для команд с многоязычным названием.
/// </summary>
public abstract record LocalizableCommand(
    string NameUz,
    string NameRu,
    string? NameEn,
    string? NameUzKiril
);
