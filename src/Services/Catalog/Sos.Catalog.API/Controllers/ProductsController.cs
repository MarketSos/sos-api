using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Catalog.Application.Commands;
using Sos.Catalog.Application.Queries;

namespace Sos.Catalog.API.Controllers;

/// <summary>
/// Mahsulotlar katalogi.
/// </summary>
[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Barcode bo'yicha mahsulot topish — kassir uchun.
    /// </summary>
    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetByBarcode(string barcode, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductByBarcodeQuery(barcode), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Yangi mahsulot yaratish.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetByBarcode), new { barcode = command.Barcode }, new { productId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Mahsulotning SKU (kirim) larini ko'rish.
    /// </summary>
    [HttpGet("{productId}/skus")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    public async Task<IActionResult> GetSkus(Guid productId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSkusByProductQuery(productId), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Mahsulotni omborga kirim qilish (yangi SKU yaratish).
    /// </summary>
    [HttpPost("{productId}/skus")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    public async Task<IActionResult> CreateSku(Guid productId, [FromBody] CreateSkuCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { ProductId = productId }, ct);
        return result.IsSuccess ? Ok(new { skuId = result.Value }) : BadRequest(new { error = result.Error });
    }
}

/// <summary>
/// O'lchov birliklari.
/// </summary>
[ApiController]
[Route("api/measurement-units")]
[Authorize]
public class MeasurementUnitsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMeasurementUnitsQuery(), ct);
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateMeasurementUnitCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }
}
