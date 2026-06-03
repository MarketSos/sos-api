namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Базовый класс для сущностей с многоязычным названием.
/// NameUz и NameRu — обязательные поля.
/// NameEn и NameUzKiril — необязательные.
/// </summary>
public abstract class LocalizableEntity<TId> : AggregateRoot<TId>
{
    /// <summary>
    /// Название на узбекском (латиница) — обязательное
    /// </summary>
    public string NameUz { get; protected set; } = default!;

    /// <summary>
    /// Название на русском — обязательное
    /// </summary>
    public string NameRu { get; protected set; } = default!;

    /// <summary>
    /// Название на английском — необязательное
    /// </summary>
    public string? NameEn { get; protected set; }

    /// <summary>
    /// Название на узбекском (кириллица) — необязательное
    /// </summary>
    public string? NameUzKiril { get; protected set; }

    /// <summary>
    /// Возвращает название по коду языка.
    /// Если перевод отсутствует — возвращает NameUz.
    /// </summary>
    public string GetName(string lang = "uz") => lang switch
    {
        "ru"       => NameRu,
        "en"       => NameEn ?? NameUz,
        "uz-kiril" => NameUzKiril ?? NameUz,
        _          => NameUz
    };

    /// <summary>
    /// Установить или обновить названия
    /// </summary>
    protected void SetNames(string nameUz, string nameRu, string? nameEn = null, string? nameUzKiril = null)
    {
        NameUz      = nameUz;
        NameRu      = nameRu;
        NameEn      = nameEn;
        NameUzKiril = nameUzKiril;
        UpdatedAt   = DateTimeOffset.UtcNow;
    }
}
