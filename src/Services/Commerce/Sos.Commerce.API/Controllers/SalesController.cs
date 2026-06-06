using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Commerce.Application.Commands;

namespace Sos.Commerce.API.Controllers;

/// <summary>
/// Kassa operatsiyalari (POS).
/// Кассовые операции (POS).
/// </summary>
[ApiController]
[Route("api/sales")]
[Authorize]
public class SalesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Yangi sotuv (chek) ochish.
    /// Открытие новой продажи (чека).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSaleCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok(new { saleId = result.Value }) : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Sotuvga mahsulot qo'shish.
    /// Добавление товара в продажу.
    /// </summary>
    [HttpPost("{saleId}/items")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem(Guid saleId, [FromBody] AddSaleItemCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { SaleId = saleId }, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Sotuvni yakunlash va to'lovni qabul qilish.
    /// Завершение продажи и приём оплаты.
    /// </summary>
    [HttpPost("{saleId}/complete")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    [ProducesResponseType<CompleteSaleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Complete(Guid saleId, [FromBody] CompleteSaleCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { SaleId = saleId }, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Sotuvni bekor qilish.
    /// Отмена продажи.
    /// </summary>
    [HttpPost("{saleId}/cancel")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(Guid saleId, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelSaleCommand(saleId), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
