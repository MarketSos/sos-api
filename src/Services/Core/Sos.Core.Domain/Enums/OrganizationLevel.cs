namespace Sos.Core.Domain.Enums;

/// <summary>
/// Tashkilot ierarxiya darajasi.
/// Root  — asosiy kompaniya (holding / bosh ofis).
/// Chain — tarmoq / mintaqa bo'limi.
/// Store — alohida do'kon / filial.
/// </summary>
public enum OrganizationLevel
{
    Root  = 1,
    Chain = 2,
    Store = 3
}
