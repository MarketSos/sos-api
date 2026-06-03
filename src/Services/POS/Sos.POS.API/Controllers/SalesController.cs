using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.POS.Application.Commands;

namespace Sos.POS.API.Controllers;

/// <summary>
/// Кассовые операции (POS).
/// Открытие чека, добавление товаров, оплата.
/// </summary>
[ApiController]
[Route("api/sales")]
[Authorize]
public class SalesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Открыть новый чек.
    /// Вызывается при начале обслуживания покупателя.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Create([FromBody] CreateSaleCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok(new { saleId = result.Value }) : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Добавить товар в открытый чек.
    /// Если товар уже есть — увеличивает количество.
    /// </summary>
    [HttpPost("{saleId}/items")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> AddItem(Guid saleId, [FromBody] AddSaleItemCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { SaleId = saleId }, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Завершить продажу и принять оплату.
    /// Формирует номер чека и вызывает событие списания со склада.
    /// </summary>
    [HttpPost("{saleId}/complete")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Complete(Guid saleId, [FromBody] CompleteSaleCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { SaleId = saleId }, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Отменить открытый чек.
    /// </summary>
    [HttpPost("{saleId}/cancel")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Cancel(Guid saleId, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelSaleCommand(saleId), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
