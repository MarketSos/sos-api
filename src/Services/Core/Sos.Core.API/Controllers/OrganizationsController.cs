using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Core.Application.Commands;
using Sos.Core.Application.Queries;
using Sos.Core.Domain.Enums;

namespace Sos.Core.API.Controllers;

/// <summary>Tashkilotlar boshqaruvi. / Управление организациями.</summary>
[ApiController]
[Route("api/organizations")]
[Authorize]
public class OrganizationsController(IMediator mediator) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMyOrganizationQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrganizationByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("by-code/{code}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrganizationByCodeQuery(code), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllOrganizationsQuery(), ct);
        return Ok(result.Value);
    }

    [HttpGet("{parentId:guid}/childs")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetChilds(Guid parentId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrganizationChildsQuery(parentId), ct);
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { organizationId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPatch("{id:guid}/names")]
    public async Task<IActionResult> UpdateNames(Guid id, [FromBody] UpdateNamesRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateOrganizationNamesCommand(id, req.NameUz, req.NameRu, req.NameEn, req.NameUzKiril), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganizationRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateOrganizationCommand(id, req.Code, req.Tin, req.Okonx, req.Oked, req.OrgType, req.IsTest), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpPatch("{id:guid}/parent")]
    public async Task<IActionResult> SetParent(Guid id, [FromBody] SetParentRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new SetOrganizationParentCommand(id, req.ParentId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new ToggleOrganizationStatusCommand(id, req.IsActive), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteOrganizationCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddMemberRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new AddMemberCommand(id, req.UserId, req.Role), ct);
        return result.IsSuccess ? Ok(new { memberId = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId, CancellationToken ct)
    {
        var result = await mediator.Send(new RemoveMemberCommand(id, userId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

// ── Request DTOs ──────────────────────────────────────────────────────────────
public record UpdateNamesRequest(string NameUz, string NameRu, string? NameEn = null, string? NameUzKiril = null);
public record UpdateOrganizationRequest(
    string?           Code    = null,
    string?           Tin     = null,
    string?           Okonx   = null,
    string?           Oked    = null,
    OrganizationType? OrgType = null,
    bool              IsTest  = false);
public record SetParentRequest(Guid? ParentId);
public record ToggleStatusRequest(bool IsActive);
public record AddMemberRequest(Guid UserId, OrganizationRole Role = OrganizationRole.Member);
