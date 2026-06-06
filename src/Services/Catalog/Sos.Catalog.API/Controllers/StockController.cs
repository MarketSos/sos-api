using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Catalog.Application.Commands;
using Sos.Catalog.Application.Queries;

namespace Sos.Catalog.API.Controllers;

/// <summary>Ombor qoldiqlari. / Складские остатки.</summary>
[ApiController]
[Route("api/stock")]
[Authorize]
public class StockController(IMediator mediator) : ControllerBase
{
    [HttpGet("{storeId}/{productId}")]
    public async Task<IActionResult> Get(Guid storeId, Guid productId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetStockQuery(productId, storeId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("{storeId}/low")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    public async Task<IActionResult> GetLow(Guid storeId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLowStockQuery(storeId), ct);
        return Ok(result.Value);
    }

    [HttpPost("add")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    public async Task<IActionResult> Add([FromBody] AddStockCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpPost("deduct")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Deduct([FromBody] DeductStockCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
