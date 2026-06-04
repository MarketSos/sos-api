using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Loyalty.Application.Commands;
using Sos.Loyalty.Application.Queries;

namespace Sos.Loyalty.API.Controllers;

[ApiController]
[Route("api/loyalty")]
[Authorize]
public class LoyaltyController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Mijozning loyallik hisobini ko'rish.
    /// </summary>
    [HttpGet("accounts/{customerId}")]
    public async Task<IActionResult> GetAccount(Guid customerId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLoyaltyAccountQuery(customerId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Mijoz uchun loyallik hisobi ochish.
    /// </summary>
    [HttpPost("accounts")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Create([FromBody] CreateLoyaltyAccountCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? Ok(new { accountId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Bonus ball yig'ish (sotuvdan keyin).
    /// </summary>
    [HttpPost("accounts/{customerId}/earn")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Earn(Guid customerId, [FromBody] EarnPointsCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { CustomerId = customerId }, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Bonus ball sarflash (to'lovda).
    /// </summary>
    [HttpPost("accounts/{customerId}/spend")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    public async Task<IActionResult> Spend(Guid customerId, [FromBody] SpendPointsCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { CustomerId = customerId }, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
