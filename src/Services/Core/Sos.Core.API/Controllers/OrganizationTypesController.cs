using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Core.Application.Commands;
using Sos.Core.Application.Queries;

namespace Sos.Core.API.Controllers;

[ApiController]
[Route("api/organization-types")]
[Authorize]
public class OrganizationTypesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<OrganizationTypeDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllOrganizationTypesQuery(), ct);
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationTypeCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganizationTypeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateOrganizationTypeCommand(id, req.NameUz, req.NameRu, req.NameEn, null, req.Icon), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteOrganizationTypeCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public record UpdateOrganizationTypeRequest(string NameUz, string NameRu, string? NameEn = null, string? Icon = null);
