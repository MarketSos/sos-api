using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sos.Commerce.Application.Commands;
using Sos.Commerce.Application.Queries;
using Sos.Commerce.Domain.Entities;

namespace Sos.Commerce.API.Controllers;

/// <summary>
/// Loyallik dasturi (ball tizimi).
/// Программа лояльности (система баллов).
/// </summary>
[ApiController]
[Route("api/loyalty")]
[Authorize]
public class LoyaltyController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Mijozning loyallik hisobini olish.
    /// Получение счёта лояльности клиента.
    /// </summary>
    [HttpGet("accounts/{customerId}")]
    [ProducesResponseType<LoyaltyAccount>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccount(Guid customerId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLoyaltyAccountQuery(customerId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Mijoz uchun loyallik hisobi ochish.
    /// Открытие счёта лояльности для клиента.
    /// </summary>
    [HttpPost("accounts")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateLoyaltyAccountCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess
            ? Ok(new { accountId = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Mijozga ball yig'ish (sotuv orqali).
    /// Начисление баллов клиенту (через продажу).
    /// </summary>
    [HttpPost("accounts/{customerId}/earn")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Earn(Guid customerId, [FromBody] EarnPointsCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { CustomerId = customerId }, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Mijozning ballarini sarflash (chegirma sifatida).
    /// Списание баллов клиента (в качестве скидки).
    /// </summary>
    [HttpPost("accounts/{customerId}/spend")]
    [Authorize(Roles = "SuperAdmin,StoreAdmin,Cashier")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Spend(Guid customerId, [FromBody] SpendPointsCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { CustomerId = customerId }, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
