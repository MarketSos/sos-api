using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Catalog.Application.Commands;
using Sos.Catalog.Application.Queries;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.API.Controllers;

/// <summary>
/// Narx qoidalari (aksiya, chegirma).
/// Правила цен (акции, скидки).
/// </summary>
[ApiController]
[Route("api/pricing")]
[Authorize]
public class PricingController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Mahsulotga tegishli faol narx qoidalarini olish.
    /// Получение активных правил цен для товара.
    /// </summary>
    [HttpGet("rules/{productId}")]
    [ProducesResponseType<List<PriceRule>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRules(Guid productId, [FromQuery] Guid? storeId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetActiveRulesQuery(productId, storeId), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Yangi narx qoidasi yaratish.
    /// Создание нового правила цены.
    /// </summary>
    [HttpPost("rules")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePriceRuleCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? Ok(new { ruleId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Narx qoidasini o'chirib qo'yish (deaktivatsiya).
    /// Деактивация правила цены.
    /// </summary>
    [HttpPost("rules/{ruleId}/deactivate")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Deactivate(Guid ruleId, CancellationToken ct)
    {
        var result = await mediator.Send(new DeactivatePriceRuleCommand(ruleId), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
