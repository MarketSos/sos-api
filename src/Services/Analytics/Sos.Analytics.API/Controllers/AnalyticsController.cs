using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Analytics.Application.Commands;
using Sos.Analytics.Application.Queries;

namespace Sos.Analytics.API.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Do'kon bo'yicha sotuv xulosasi (daromad, sotuvlar soni, o'rtacha chek).
    /// </summary>
    [HttpGet("sales/summary")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Analyst")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] Guid storeId,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetSalesSummaryQuery(storeId, from, to), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Barcha do'konlar bo'yicha daromad taqqoslamasi.
    /// </summary>
    [HttpGet("sales/revenue-by-store")]
    [Authorize(Roles = "SuperAdmin,Analyst")]
    public async Task<IActionResult> GetRevenueByStore(
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetRevenueByStoreQuery(from, to), ct);
        return Ok(result.Value.Select(x => new { storeId = x.StoreId, revenue = x.Revenue }));
    }

    /// <summary>
    /// Sotuv ma'lumotini yozib olish (POS servisidan chaqiriladi).
    /// </summary>
    [HttpPost("sales")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Record([FromBody] RecordSaleCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
