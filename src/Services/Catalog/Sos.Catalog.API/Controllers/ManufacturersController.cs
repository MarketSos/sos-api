using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Catalog.Application.Commands;
using Sos.Catalog.Application.Queries;

namespace Sos.Catalog.API.Controllers;

/// <summary>
/// Ишлаб чиқарувчилар (manufacturer) бошқаруви.
/// Управление производителями.
/// </summary>
[ApiController]
[Route("api/manufacturers")]
[Authorize]
public class ManufacturersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Barcha ishlab chiqaruvchilar ro'yxati.
    /// Список всех производителей.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<List<ManufacturerDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllManufacturersQuery(), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Ishlab chiqaruvchini ID bo'yicha olish.
    /// Получение производителя по ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<ManufacturerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetManufacturerByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Yangi ishlab chiqaruvchi yaratish.
    /// Создание нового производителя.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateManufacturerCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { manufacturerId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Ishlab chiqaruvchi ma'lumotlarini yangilash.
    /// Обновление данных производителя.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateManufacturerRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateManufacturerCommand(id, req.Code, req.NameUz, req.NameRu, req.NameEn, req.NameUzCyrl, req.NameKk, req.AddressLine, req.Phone), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Ishlab chiqaruvchini o'chirish (soft delete).
    /// Удаление производителя (мягкое удаление).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteManufacturerCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public record UpdateManufacturerRequest(
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzCyrl = null,
    string? NameKk      = null,
    string? AddressLine = null,
    string? Phone       = null);
