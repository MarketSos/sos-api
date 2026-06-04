using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Pricing.Application.Commands;
using Sos.Pricing.Application.Queries;

namespace Sos.Pricing.API.Controllers;

[ApiController]
[Route("api/pricing")]
[Authorize]
public class PricingController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Mahsulot uchun amaldagi narx qoidalarini olish.
    /// </summary>
    [HttpGet("rules/{productId}")]
    public async Task<IActionResult> GetRules(Guid productId, [FromQuery] Guid? storeId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetActiveRulesQuery(productId, storeId), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Yangi narx qoidasi yaratish (aksiya, chegirma).
    /// </summary>
    [HttpPost("rules")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    public async Task<IActionResult> Create([FromBody] CreatePriceRuleCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? Ok(new { ruleId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Narx qoidasini o'chirish (deactivate).
    /// </summary>
    [HttpPost("rules/{ruleId}/deactivate")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    public async Task<IActionResult> Deactivate(Guid ruleId, CancellationToken ct)
    {
        var result = await mediator.Send(new DeactivatePriceRuleCommand(ruleId), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
