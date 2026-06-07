namespace Sos.Core.Domain.Enums;

/// <summary>
/// Mulkchilik turi (tizim yoki mijoz tashkiloti).
/// Ownership Type (system or customer organization).
/// System   — tizim ichki tashkiloti (boshqaruv markazi).
/// Customer — mijoz tashkiloti (do'kon, apteka va h.k.).
/// </summary>
public enum OwnershipType
{
    Customer = 1,
    System   = 2
}
