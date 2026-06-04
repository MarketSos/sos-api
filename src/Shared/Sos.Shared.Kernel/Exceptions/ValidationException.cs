namespace Sos.Shared.Kernel.Exceptions;

/// <summary>
/// FluentValidation pipeline-dan keladigan xato.
/// Request handler-ga yetib kelmasdan, MediatR pipeline-da ushlanadi.
/// HTTP 400 Bad Request qaytaradi.
///
/// Qachon ishlatiladi: input formati / majburiy maydonlar xatoligi.
/// Qachon ISHLATILMAYDI: biznes qoidalari buzilganda — u holda Result.Failure ishlating.
/// </summary>
public sealed class ValidationException(IEnumerable<ValidationError> errors)
    : Exception("Bir yoki bir nechta validatsiya xatolari topildi.")
{
    public IReadOnlyList<ValidationError> Errors { get; } = errors.ToList().AsReadOnly();

    public ValidationException(string field, string message)
        : this([new ValidationError(field, message)]) { }
}

/// <param name="Field">Xatolik bo'lgan maydon nomi (camelCase, masalan "email")</param>
/// <param name="Message">Foydalanuvchiga ko'rsatiladigan xabar</param>
public record ValidationError(string Field, string Message);
