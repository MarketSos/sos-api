namespace Sos.Shared.Infrastructure.Services;

/// <summary>
/// Сервис текущего пользователя — извлекает ID из JWT-токена.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// ID текущего авторизованного пользователя (null для анонимных запросов)
    /// </summary>
    public Guid? UserId { get; }

    /// <summary>
    /// Авторизован ли текущий пользователь
    /// </summary>
    public bool IsAuthenticated { get; }
}
