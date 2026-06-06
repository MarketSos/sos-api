using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Core.Application.Commands;
using Sos.Core.Application.Queries;
using Sos.Shared.Kernel.Domain;

namespace Sos.Core.API.Controllers;

/// <summary>
/// Xodimlarni boshqarish.
/// Управление сотрудниками.
/// </summary>
[ApiController]
[Route("api/employees")]
[Authorize]
public class EmployeesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Barcha xodimlar ro'yxati.
    /// Список всех сотрудников.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType<List<EmployeeSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string lang = "uz", CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllEmployeesQuery(lang), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Joriy foydalanuvchining xodim profili.
    /// Профиль сотрудника текущего пользователя.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType<EmployeeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe([FromQuery] string lang = "uz", CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetMyEmployeeQuery(lang), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Xodimni ID bo'yicha olish.
    /// Получение сотрудника по ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<EmployeeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] string lang = "uz", CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetEmployeeByIdQuery(id, lang), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Yangi xodim yaratish.
    /// Создание нового сотрудника.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { employeeId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Xodim ma'lumotlarini yangilash.
    /// Обновление данных сотрудника.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateEmployeeCommand(
            id, req.FirstName, req.LastName, req.MiddleName,
            req.BirthDate, req.Phone, req.Gender,
            req.SpecializationId, req.EmployeeRankId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Xodimni ishga qabul qilish sanasini belgilash.
    /// Установка даты приёма сотрудника на работу.
    /// </summary>
    [HttpPatch("{id:guid}/hire")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Hire(Guid id, [FromBody] HireRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new HireEmployeeCommand(id, req.HireDate), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Xodimni ishdan bo'shatish.
    /// Увольнение сотрудника.
    /// </summary>
    [HttpPatch("{id:guid}/fire")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Fire(Guid id, [FromBody] FireRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new FireEmployeeCommand(id, req.FireDate), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Xodimni o'chirish (soft delete).
    /// Удаление сотрудника (мягкое удаление).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteEmployeeCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

// ── Request DTOs ──────────────────────────────────────────────────────────────
public record UpdateEmployeeRequest(
    string    FirstName,
    string    LastName,
    string?   MiddleName       = null,
    DateOnly? BirthDate        = null,
    string?   Phone            = null,
    Gender?   Gender           = null,
    Guid?     SpecializationId = null,
    Guid?     EmployeeRankId   = null);

public record HireRequest(DateOnly HireDate);
public record FireRequest(DateOnly FireDate);
