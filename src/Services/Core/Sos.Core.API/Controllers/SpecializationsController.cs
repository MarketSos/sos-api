using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Core.Application.Commands;
using Sos.Core.Application.Queries;

namespace Sos.Core.API.Controllers;

/// <summary>
/// Mutaxassisliklar (kasb turlari) boshqaruvi.
/// Управление специализациями (видами должностей).
/// </summary>
[ApiController]
[Route("api/specializations")]
[Authorize]
public class SpecializationsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Barcha mutaxassisliklar ro'yxati.
    /// Список всех специализаций.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<List<SpecializationDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllSpecializationsQuery(), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Mutaxassislikni ID bo'yicha olish.
    /// Получение специализации по ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<SpecializationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSpecializationByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Yangi mutaxassislik yaratish.
    /// Создание новой специализации.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSpecializationCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { specializationId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Mutaxassislik ma'lumotlarini yangilash.
    /// Обновление данных специализации.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSpecializationRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateSpecializationCommand(id, req.Code, req.NameUz, req.NameRu, req.NameEn, req.NameUzCyrl, req.NameKk), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Mutaxassislikni o'chirish (soft delete).
    /// Удаление специализации (мягкое удаление).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteSpecializationCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public record UpdateSpecializationRequest(
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzCyrl = null,
    string? NameKk      = null);
