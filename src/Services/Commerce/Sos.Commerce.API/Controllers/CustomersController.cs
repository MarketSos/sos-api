using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Commerce.Application.Commands;
using Sos.Commerce.Application.Queries;

namespace Sos.Commerce.API.Controllers;

/// <summary>Mijozlar boshqaruvi. / Управление покупателями.</summary>
[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new SearchCustomersQuery(q, limit), ct);
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { customerId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { CustomerId = id }, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
