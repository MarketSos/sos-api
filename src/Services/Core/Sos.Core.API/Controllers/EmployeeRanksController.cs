using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Core.Application.Commands;
using Sos.Core.Application.Queries;

namespace Sos.Core.API.Controllers;

/// <summary>
/// Xodim darajalari (lavozimlari) boshqaruvi.
/// Управление должностными рангами сотрудников.
/// </summary>
[ApiController]
[Route("api/employee-ranks")]
[Authorize]
public class EmployeeRanksController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Barcha xodim darajalari ro'yxati.
    /// Список всех рангов сотрудников.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<List<EmployeeRankDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllEmployeeRanksQuery(), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Xodim darajasini ID bo'yicha olish.
    /// Получение ранга сотрудника по ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<EmployeeRankDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetEmployeeRankByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Yangi xodim darajasi yaratish.
    /// Создание нового ранга сотрудника.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRankCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { employeeRankId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Xodim darajasi ma'lumotlarini yangilash.
    /// Обновление данных ранга сотрудника.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRankRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateEmployeeRankCommand(id, req.Code, req.NameUz, req.NameRu, req.NameEn, req.NameUzKiril), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Xodim darajasini o'chirish (soft delete).
    /// Удаление ранга сотрудника (мягкое удаление).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteEmployeeRankCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public record UpdateEmployeeRankRequest(
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzKiril = null);
