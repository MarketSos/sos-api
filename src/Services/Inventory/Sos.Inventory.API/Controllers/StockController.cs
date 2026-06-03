using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Inventory.Application.Commands;
using Sos.Inventory.Application.Queries;

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
    /// Получить остаток товара на складе.
    /// </summary>
    [HttpGet("{storeId}/{productId}")]
    public async Task<IActionResult> Get(Guid storeId, Guid productId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetStockQuery(productId, storeId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Товары с запасом ниже минимального — для уведомлений о дозаказе.
    /// </summary>
    [HttpGet("{storeId}/low")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    public async Task<IActionResult> GetLow(Guid storeId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLowStockQuery(storeId), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Принять товар на склад (поступление или инвентаризация).
    /// </summary>
    [HttpPost("add")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    public async Task<IActionResult> Add([FromBody] AddStockCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

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
