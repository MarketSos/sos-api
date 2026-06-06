using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Commerce.Application.Commands;

namespace Sos.Commerce.API.Controllers;

/// <summary>Kassa operatsiyalari (POS). / Кассовые операции.</summary>
[ApiController]
[Route("api/sales")]
[Authorize]
public class SalesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Create([FromBody] CreateSaleCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok(new { saleId = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{saleId}/items")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> AddItem(Guid saleId, [FromBody] AddSaleItemCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { SaleId = saleId }, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{saleId}/complete")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Complete(Guid saleId, [FromBody] CompleteSaleCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { SaleId = saleId }, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{saleId}/cancel")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Cancel(Guid saleId, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelSaleCommand(saleId), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
