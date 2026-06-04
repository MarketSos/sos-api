namespace Sos.Shared.Kernel.Exceptions;

/// <summary>
/// Agregat yoki entity ID bo'yicha MAVJUD BO'LISHI KERAK bo'lgan holda topilmasa.
/// HTTP 404 Not Found qaytaradi.
///
/// Qachon ishlatiladi: foreign key relation buzilganda, yoki
/// ID orqali fetch qilishda (controller yoki infra qatlami).
///
/// Qachon ISHLATILMAYDI: oddiy "topilmadi" biznes holati —
/// u holda Result.Failure ishlating (masalan, barcode bo'yicha qidiruv).
/// </summary>
public sealed class NotFoundException(string entityName, object id)
    : Exception($"'{entityName}' (id: {id}) topilmadi.")
{
    public string EntityName { get; } = entityName;
    public object Id { get; } = id;
}
