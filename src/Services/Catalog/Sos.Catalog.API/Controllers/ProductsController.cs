using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Catalog.Application.Commands;
using Sos.Catalog.Application.Queries;

namespace Sos.Catalog.API.Controllers;

/// <summary>
/// Управление товарами каталога.
/// Поиск по штрих-коду, создание и редактирование товаров.
/// </summary>
[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Найти товар по штрих-коду.
    /// Используется кассиром при сканировании товара.
    /// </summary>
    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetByBarcode(string barcode, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductByBarcodeQuery(barcode), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Создать новый товар в каталоге.
    /// Доступно только администраторам.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetByBarcode), new { id = result.Value }, new { productId = result.Value })
            : BadRequest(new { error = result.Error });
    }
}
