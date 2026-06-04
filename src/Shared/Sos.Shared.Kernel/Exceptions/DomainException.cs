namespace Sos.Shared.Kernel.Exceptions;

/// <summary>
/// Domain invariant (o'zgarmas qoida) buzilganda domain entity-dan tashlangan xato.
/// HTTP 422 Unprocessable Entity qaytaradi.
///
/// Qachon ishlatiladi: HECH QACHON bo'lmasligi kerak bo'lgan holat
/// (masalan: manfiy balans, noto'g'ri state transition, biznes qoidasi buzilishi).
///
/// Qachon ISHLATILMAYDI: kutilgan biznes holatlari —
/// u holda Result.Failure ishlating.
///
/// Misol: LoyaltyAccount.Spend() — balans yetarli emasligi domain qoidasi.
/// </summary>
public sealed class DomainException(string message) : Exception(message);
