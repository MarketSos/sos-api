using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Core.Application.Commands;

namespace Sos.Core.API.Controllers;

/// <summary>
/// Autentifikatsiya va token boshqaruvi.
/// Аутентификация и управление токенами.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Ro'yxatdan o'tish va avtomatik kirish.
    /// Регистрация с автовходом.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var registerResult = await mediator.Send(command, ct);
        if (!registerResult.IsSuccess)
            return BadRequest(new { error = registerResult.Error });

        var loginResult = await mediator.Send(new LoginCommand(command.UserName, command.Password), ct);
        return loginResult.IsSuccess
            ? Ok(loginResult.Value)
            : BadRequest(new { error = loginResult.Error });
    }

    /// <summary>
    /// Login va parol bilan kirish.
    /// Вход по логину и паролю.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { error = result.Error });
    }

    /// <summary>
    /// Access token yangilash.
    /// Обновление access-токена.
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { error = result.Error });
    }

    /// <summary>
    /// Tizimdan chiqish.
    /// Выход из системы.
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct)
    {
        await mediator.Send(command, ct);
        return NoContent();
    }

    /// <summary>
    /// Joriy foydalanuvchi ma'lumotlari (token orqali).
    /// Данные текущего пользователя (по токену).
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
        => Ok(User.Claims.Select(c => new { c.Type, c.Value }));
}
