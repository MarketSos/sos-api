using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Identity.Application.Commands;

namespace Sos.Identity.API.Controllers;

/// <summary>
/// Авторизация и управление токенами.
/// Все операции входа, регистрации и обновления токенов.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Регистрация нового пользователя.
    /// После создания автоматически выполняет вход и возвращает токены.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var registerResult = await mediator.Send(command, ct);
        if (!registerResult.IsSuccess)
            return BadRequest(new { error = registerResult.Error });

        var loginResult = await mediator.Send(
            new LoginCommand { Email = command.Email, Password = command.Password }, ct);

        return loginResult.IsSuccess
            ? Ok(loginResult.Value)
            : BadRequest(new { error = loginResult.Error });
    }

    /// <summary>
    /// Вход в систему по email и паролю.
    /// Возвращает access-токен и refresh-токен.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { error = result.Error });
    }

    /// <summary>
    /// Обновление access-токена по refresh-токену.
    /// Старый refresh-токен аннулируется, выдаётся новая пара.
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { error = result.Error });
    }

    /// <summary>
    /// Текущий авторизованный пользователь.
    /// Возвращает данные из JWT-токена.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}
