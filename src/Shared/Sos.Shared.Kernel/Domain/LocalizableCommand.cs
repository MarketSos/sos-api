namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Базовая запись для команд с многоязычным названием.
/// </summary>
public abstract record LocalizableCommand(
    string NameUz,
    string NameUzCyrl,
    string NameRu,
    string? NameEn,
    string? NameKk
);
