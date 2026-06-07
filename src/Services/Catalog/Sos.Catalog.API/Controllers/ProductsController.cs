using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Catalog.Application.Commands;
using Sos.Catalog.Application.Queries;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.API.Controllers;

/// <summary>
/// Mahsulotlar katalogi.
/// Каталог товаров.
/// </summary>
[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Mahsulotlar ro'yxati — qidiruv yoki kategoriya bo'yicha.
    /// Список товаров — поиск или фильтр по категории.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<Product>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] Guid? categoryId, CancellationToken ct)
    {
        var result = await mediator.Send(new SearchProductsQuery(q, categoryId), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Barcode bo'yicha mahsulot topish — kassir uchun.
    /// Поиск товара по штрихкоду — для кассира.
    /// </summary>
    [HttpGet("barcode/{barcode}")]
    [ProducesResponseType<Product>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByBarcode(string barcode, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductByBarcodeQuery(barcode), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Yangi mahsulot yaratish.
    /// Создание нового товара.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetByBarcode), new { barcode = command.Barcode }, new { productId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Mahsulotning SKU (kirim partiyalari) ro'yxatini ko'rish.
    /// Просмотр SKU (партий поступления) товара.
    /// </summary>
    [HttpGet("{productId}/skus")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType<IEnumerable<Sku>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkus(Guid productId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSkusByProductQuery(productId), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Mahsulotni omborga kirim qilish (yangi SKU yaratish).
    /// Оприходование товара на склад (создание нового SKU).
    /// </summary>
    [HttpPost("{productId}/skus")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSku(Guid productId, [FromBody] CreateSkuCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { ProductId = productId }, ct);
        return result.IsSuccess ? Ok(new { skuId = result.Value }) : BadRequest(new { error = result.Error });
    }
}

/// <summary>
/// O'lchov birliklari boshqaruvi.
/// Управление единицами измерения.
/// </summary>
[ApiController]
[Route("api/measurement-units")]
[Authorize]
public class MeasurementUnitsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Barcha o'lchov birliklari ro'yxati.
    /// Список всех единиц измерения.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<MeasurementUnit>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMeasurementUnitsQuery(), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Yangi o'lchov birligi yaratish.
    /// Создание новой единицы измерения.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMeasurementUnitCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }
}

/// <summary>
/// Kategoriyalar ro'yxati.
/// Список категорий товаров.
/// </summary>
[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Barcha kategoriyalar (id, nameUz, nameRu, parentId).
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<CategoryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCategoriesQuery(), ct);
        return Ok(result.Value);
    }
}
