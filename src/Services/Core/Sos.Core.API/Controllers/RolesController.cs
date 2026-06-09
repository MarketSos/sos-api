using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Core.Application.Commands;
using Sos.Core.Application.Queries;

namespace Sos.Core.API.Controllers;

/// <summary>
/// Rollar va permissionlarni boshqarish.
/// Управление ролями и разрешениями.
/// </summary>
[ApiController]
[Route("api/roles")]
[Authorize(Roles = "SuperAdmin")]
public class RolesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Barcha rollar ro'yxati (permission soni bilan).
    /// Список всех ролей (с количеством разрешений).
    /// </summary>
    [HttpGet]
    [ProducesResponseType<List<RoleSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllRolesQuery(), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Rolni nomi bo'yicha olish (barcha permissionlar bilan).
    /// Получение роли по имени (со всеми разрешениями).
    /// </summary>
    [HttpGet("{name}")]
    [ProducesResponseType<RoleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName(string name, CancellationToken ct)
    {
        var result = await mediator.Send(new GetRoleByNameQuery(name), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Yangi rol yaratish.
    /// Создание новой роли.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CreateRoleCommand(req.Name, req.NameUz, req.NameUzCyrl, req.NameRu, req.NameEn, req.NameKk), ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetByName), new { name = req.Name }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Rol nomlarini yangilash.
    /// Обновление названий роли.
    /// </summary>
    [HttpPut("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(string name, [FromBody] UpdateRoleRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateRoleCommand(name, req.NameUz, req.NameUzCyrl, req.NameRu, req.NameEn, req.NameKk), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Rolni o'chirish.
    /// Удаление роли.
    /// </summary>
    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(string name, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteRoleCommand(name), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Rolga yangi permission qo'shish.
    /// Добавление нового разрешения к роли.
    /// </summary>
    [HttpPost("{name}/permissions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPermission(
        string name,
        [FromBody] PermissionRequest req,
        CancellationToken ct)
    {
        var result = await mediator.Send(new AddRolePermissionCommand(name, req.Permission), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Roldan permission olib tashlash.
    /// Удаление разрешения из роли.
    /// </summary>
    [HttpDelete("{name}/permissions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemovePermission(
        string name,
        [FromBody] PermissionRequest req,
        CancellationToken ct)
    {
        var result = await mediator.Send(new RemoveRolePermissionCommand(name, req.Permission), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public record CreateRoleRequest(string Name, string NameUz, string NameUzCyrl, string NameRu, string? NameEn, string? NameKk);
public record UpdateRoleRequest(string NameUz, string NameUzCyrl, string NameRu, string? NameEn, string? NameKk);
public record PermissionRequest(string Permission);
