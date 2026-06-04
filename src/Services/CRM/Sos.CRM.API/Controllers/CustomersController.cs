using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.CRM.Application.Commands;
using Sos.CRM.Application.Queries;

namespace Sos.CRM.API.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Mijozni ID bo'yicha topish.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Mijozlarni qidirish (ism, familya, telefon).
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new SearchCustomersQuery(q, limit), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Yangi mijoz yaratish.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { customerId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Mijoz ma'lumotlarini yangilash.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { CustomerId = id }, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
