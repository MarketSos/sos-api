using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.POS.Application.Commands;

namespace Sos.POS.API.Controllers;

[ApiController]
[Route("api/sales")]
[Authorize]
public class SalesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok(new { saleId = result.Value }) : BadRequest(result.Error);
    }

    [HttpPost("{saleId}/items")]
    public async Task<IActionResult> AddItem(Guid saleId, [FromBody] AddSaleItemCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { SaleId = saleId }, ct);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("{saleId}/complete")]
    public async Task<IActionResult> Complete(Guid saleId, [FromBody] CompleteSaleCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { SaleId = saleId }, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
