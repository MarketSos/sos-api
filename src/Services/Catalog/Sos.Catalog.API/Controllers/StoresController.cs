using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Catalog.Application.Commands;
using Sos.Catalog.Application.Queries;

namespace Sos.Catalog.API.Controllers;

public record UpdateStoreRequest(
    string  Code,
    string  Name,
    string? Address = null,
    string? Phone   = null);

/// <summary>
/// Do'konlar boshqaruvi.
/// Управление магазинами.
/// </summary>
[ApiController]
[Route("api/stores")]
[Authorize]
public class StoresController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Tashkilot do'konlari ro'yxati.
    /// Список магазинов организации.
    /// </summary>
    [HttpGet("by-organization/{organizationId:guid}")]
    [ProducesResponseType<List<StoreDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByOrganization(Guid organizationId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetStoresByOrganizationQuery(organizationId), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Do'konni ID bo'yicha olish.
    /// Получение магазина по ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<StoreDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetStoreByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Yangi do'kon yaratish.
    /// Создание нового магазина.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateStoreCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { storeId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Do'kon ma'lumotlarini yangilash.
    /// Обновление данных магазина.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStoreRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateStoreCommand(id, req.Code, req.Name, req.Address, req.Phone), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Do'konni faollashtirish.
    /// Активация магазина.
    /// </summary>
    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ActivateStoreCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Do'konni o'chirish (deactivate).
    /// Деактивация магазина.
    /// </summary>
    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeactivateStoreCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Do'konni o'chirish (soft delete).
    /// Удаление магазина (мягкое удаление).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteStoreCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
