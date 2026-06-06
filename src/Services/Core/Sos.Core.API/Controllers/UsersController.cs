using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Core.Application.Commands;
using Sos.Core.Application.Queries;

namespace Sos.Core.API.Controllers;

/// <summary>
/// Foydalanuvchilarni boshqarish.
/// Управление пользователями.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize(Roles = "SuperAdmin")]
public class UsersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Barcha foydalanuvchilar ro'yxati.
    /// Список всех пользователей.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<List<UserDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllUsersQuery(), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Foydalanuvchini ID bo'yicha ko'rish.
    /// Получение пользователя по ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Foydalanuvchi rolini o'zgartirish.
    /// Изменение роли пользователя.
    /// </summary>
    [HttpPatch("{id:guid}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeRole(
        Guid id,
        [FromBody] ChangeRoleRequest req,
        CancellationToken ct)
    {
        var result = await mediator.Send(new ChangeUserRoleCommand(id, req.RoleIds), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Foydalanuvchini do'konga biriktirish (null — do'kondan chiqarish).
    /// Привязка пользователя к магазину (null — открепить).
    /// </summary>
    [HttpPatch("{id:guid}/store")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignStore(
        Guid id,
        [FromBody] AssignStoreRequest req,
        CancellationToken ct)
    {
        var result = await mediator.Send(new AssignUserToStoreCommand(id, req.StoreId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Foydalanuvchi parolini yangilash (o'zi uchun).
    /// Смена пароля пользователем (для себя).
    /// </summary>
    [HttpPatch("{id:guid}/password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        Guid id,
        [FromBody] ChangePasswordRequest req,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new ChangeUserPasswordCommand(id, req.CurrentPassword, req.NewPassword), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Foydalanuvchi parolini tiklash — eski parol talab qilinmaydi (SuperAdmin).
    /// Сброс пароля пользователя без старого пароля (SuperAdmin).
    /// </summary>
    [HttpPatch("{id:guid}/password/reset")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        Guid id,
        [FromBody] ResetPasswordRequest req,
        CancellationToken ct)
    {
        var result = await mediator.Send(new ResetUserPasswordCommand(id, req.NewPassword), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Foydalanuvchini faollashtirish.
    /// Активация пользователя.
    /// </summary>
    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ActivateUserCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Foydalanuvchini bloklash.
    /// Блокировка пользователя.
    /// </summary>
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeactivateUserCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public record ChangeRoleRequest(Guid[] RoleIds);
public record AssignStoreRequest(Guid? StoreId);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record ResetPasswordRequest(string NewPassword);
