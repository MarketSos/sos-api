using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Commerce.Application.Commands;
using Sos.Commerce.Application.Queries;
using Sos.Commerce.Domain.Entities;

namespace Sos.Commerce.API.Controllers;

/// <summary>
/// Mijozlar boshqaruvi.
/// Управление покупателями.
/// </summary>
[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Mijozni ID bo'yicha olish.
    /// Получение покупателя по ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType<Customer>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Mijozlarni ism/telefon bo'yicha qidirish.
    /// Поиск покупателей по имени или телефону.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType<List<Customer>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new SearchCustomersQuery(q, limit), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Yangi mijoz yaratish.
    /// Создание нового покупателя.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { customerId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Mijoz ma'lumotlarini yangilash.
    /// Обновление данных покупателя.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { CustomerId = id }, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
