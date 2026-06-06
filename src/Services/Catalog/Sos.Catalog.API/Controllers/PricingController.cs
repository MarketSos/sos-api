using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Catalog.Application.Commands;
using Sos.Catalog.Application.Queries;

namespace Sos.Catalog.API.Controllers;

/// <summary>Narx qoidalari (aksiya, chegirma). / Правила цен.</summary>
[ApiController]
[Route("api/pricing")]
[Authorize]
public class PricingController(IMediator mediator) : ControllerBase
{
    [HttpGet("rules/{productId}")]
    public async Task<IActionResult> GetRules(Guid productId, [FromQuery] Guid? storeId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetActiveRulesQuery(productId, storeId), ct);
        return Ok(result.Value);
    }

    [HttpPost("rules")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    public async Task<IActionResult> Create([FromBody] CreatePriceRuleCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? Ok(new { ruleId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("rules/{ruleId}/deactivate")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    public async Task<IActionResult> Deactivate(Guid ruleId, CancellationToken ct)
    {
        var result = await mediator.Send(new DeactivatePriceRuleCommand(ruleId), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
