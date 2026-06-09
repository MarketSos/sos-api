using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Core.Application.Commands;
using Sos.Core.Application.Queries;
using Sos.Core.Domain.Enums;

namespace Sos.Core.API.Controllers;

/// <summary>
/// Tashkilotlar boshqaruvi.
/// Управление организациями.
/// </summary>
[ApiController]
[Route("api/organizations")]
[Authorize]
public class OrganizationsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Joriy foydalanuvchining tashkiloti.
    /// Организация текущего пользователя.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType<OrganizationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMyOrganizationQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Tashkilotni ID bo'yicha olish.
    /// Получение организации по ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType<OrganizationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrganizationByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Tashkilotni kod bo'yicha olish.
    /// Получение организации по коду.
    /// </summary>
    [HttpGet("by-code/{code}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType<OrganizationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrganizationByCodeQuery(code), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Barcha tashkilotlar ro'yxati.
    /// Список всех организаций.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType<List<OrganizationSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllOrganizationsQuery(), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Tashkilotning pastki bo'linmalarini olish.
    /// Получение дочерних подразделений организации.
    /// </summary>
    [HttpGet("{parentId:guid}/childs")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType<List<OrganizationSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChilds(Guid parentId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrganizationChildsQuery(parentId), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Yangi tashkilot yaratish.
    /// Создание новой организации.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { organizationId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Tashkilot nomlarini yangilash (uz/ru/en/kiril).
    /// Обновление названий организации (uz/ru/en/kiril).
    /// </summary>
    [HttpPatch("{id:guid}/names")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateNames(Guid id, [FromBody] UpdateNamesRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateOrganizationNamesCommand(id, req.NameUz, req.NameRu, req.NameEn, req.NameUzCyrl, req.NameKk), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Tashkilot asosiy ma'lumotlarini yangilash.
    /// Обновление основных реквизитов организации.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganizationRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateOrganizationCommand(id, req.Code, req.Tin, req.Okonx, req.Oked, req.IsTest), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Tashkilotning yuqori tashkilotini (parent) o'rnatish.
    /// Установка родительской организации.
    /// </summary>
    [HttpPatch("{id:guid}/parent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetParent(Guid id, [FromBody] SetParentRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new SetOrganizationParentCommand(id, req.ParentId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Tashkilot faollik holatini o'zgartirish.
    /// Изменение статуса активности организации.
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new ToggleOrganizationStatusCommand(id, req.IsActive), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Tashkilotni o'chirish (soft delete).
    /// Удаление организации (мягкое удаление).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteOrganizationCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Tashkilot tur (System/Customer) va darajasini (Root/Chain/Store) o'rnatish.
    /// Установка типа и уровня организации.
    /// </summary>
    [HttpPatch("{id:guid}/classification")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Classify(Guid id, [FromBody] ClassifyRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new ClassifyOrganizationCommand(id, req.Ownership, req.Level), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Tashkilotga a'zo qo'shish.
    /// Добавление участника в организацию.
    /// </summary>
    [HttpPost("{id:guid}/members")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddMemberRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new AddMemberCommand(id, req.UserId, req.Role), ct);
        return result.IsSuccess ? Ok(new { memberId = result.Value }) : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Tashkilotdan a'zoni chiqarish.
    /// Удаление участника из организации.
    /// </summary>
    [HttpDelete("{id:guid}/members/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId, CancellationToken ct)
    {
        var result = await mediator.Send(new RemoveMemberCommand(id, userId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

// ── Request DTOs ──────────────────────────────────────────────────────────────
public record UpdateNamesRequest(string NameUz, string NameRu, string? NameEn = null, string? NameUzCyrl = null, string? NameKk = null);
public record ClassifyRequest(OwnershipType Ownership, OrganizationLevel Level);
public record UpdateOrganizationRequest(
    string? Code   = null,
    string? Tin    = null,
    string? Okonx  = null,
    string? Oked   = null,
    bool    IsTest = false);
public record SetParentRequest(Guid? ParentId);
public record ToggleStatusRequest(bool IsActive);
public record AddMemberRequest(Guid UserId, OrganizationRole Role = OrganizationRole.Member);
