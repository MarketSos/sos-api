using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Catalog.Application.Commands;
using Sos.Catalog.Application.Queries;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.API.Controllers;

/// <summary>
/// Ombor qoldiqlari boshqaruvi.
/// Управление складскими остатками.
/// </summary>
[ApiController]
[Route("api/stock")]
[Authorize]
public class StockController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Do'kon va mahsulot bo'yicha ombor qoldiqlarini olish.
    /// Получение остатков по магазину и товару.
    /// </summary>
    [HttpGet("{storeId}/{productId}")]
    [ProducesResponseType<StockItem>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid storeId, Guid productId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetStockQuery(productId, storeId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Do'kon bo'yicha kam qolgan mahsulotlar ro'yxati.
    /// Список товаров с низким остатком по магазину.
    /// </summary>
    [HttpGet("{storeId}/low")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType<IEnumerable<StockItem>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLow(Guid storeId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLowStockQuery(storeId), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Omborga mahsulot qo'shish (kirim).
    /// Добавление товара на склад (приход).
    /// </summary>
    [HttpPost("add")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] AddStockCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Ombordan mahsulot chiqarish (sotuv/sarflov).
    /// Списание товара со склада (продажа/расход).
    /// </summary>
    [HttpPost("deduct")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Deduct([FromBody] DeductStockCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
