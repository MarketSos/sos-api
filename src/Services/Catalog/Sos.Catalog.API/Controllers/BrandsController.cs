using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Catalog.Application.Commands;
using Sos.Catalog.Application.Queries;

namespace Sos.Catalog.API.Controllers;

/// <summary>
/// Брендлар бошқаруви.
/// Управление брендами.
/// </summary>
[ApiController]
[Route("api/brands")]
[Authorize]
public class BrandsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Barcha brendlar ro'yxati.
    /// Список всех брендов.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<List<BrandDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllBrandsQuery(), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Brendni ID bo'yicha olish.
    /// Получение бренда по ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<BrandDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetBrandByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Yangi brend yaratish.
    /// Создание нового бренда.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateBrandCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { brandId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Brend ma'lumotlarini yangilash.
    /// Обновление данных бренда.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrandRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateBrandCommand(id, req.Code, req.NameUz, req.NameRu, req.NameEn, req.NameUzKiril), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Brendni o'chirish (soft delete).
    /// Удаление бренда (мягкое удаление).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteBrandCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public record UpdateBrandRequest(
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzKiril = null);
