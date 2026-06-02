using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Inventory.Application.Commands;

namespace Sos.Inventory.API.Controllers;

[ApiController]
[Route("api/stock")]
[Authorize]
public class StockController(IMediator mediator) : ControllerBase
{
    [HttpPost("deduct")]
    public async Task<IActionResult> Deduct([FromBody] DeductStockCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
