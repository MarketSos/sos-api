using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Inventory.Application.Commands;

namespace Sos.Inventory.API.Controllers;

/// <summary>
/// Управление складскими остатками.
/// Списание, поступление и контроль минимального запаса.
/// </summary>
[ApiController]
[Route("api/stock")]
[Authorize]
public class StockController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Списать товар со склада.
    /// Вызывается автоматически при завершении продажи (POS → Inventory).
    /// </summary>
    [HttpPost("deduct")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Deduct([FromBody] DeductStockCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
