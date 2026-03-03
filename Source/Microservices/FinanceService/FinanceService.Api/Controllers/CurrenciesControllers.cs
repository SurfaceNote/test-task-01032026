using FinanceService.Api.Extenstions;
using FinanceService.Application.Commands;
using FinanceService.Application.Queries;
using FinanceService.Contracts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/currencies")]
public class CurrenciesControllers : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetAllCurrenciesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetAll([FromServices] GetAllCurrenciesQueryHandler handler, CancellationToken cancellationToken)
    {
        var currencies = await handler.Handle(new GetAllCurrenciesQuery(), cancellationToken);
        
        // По хорошему. тут бы использовать либо автомаппер, либо написать свой маппер
        var currenciesDto = new GetAllCurrenciesResponse
        {
            Currencies = currencies.OrderBy(c => c.CharCode).Select(c => new CurrencyDto
            {
                Id = c.Id,
                CharCode = c.CharCode,
                Rate = c.Rate,
                UpdatedAt = c.UpdatedAt,

            }).ToList()
        };
        return Ok(currenciesDto);
    }

    [HttpPost("favorites")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteCurrencyRequest request,
        [FromServices] AddFavoriteCurrencyCommandHandler handler, CancellationToken cancellationToken)
    {
        var command = new AddFavoriteCurrencyCommand
        {
            UserId = User.GetRequiredUserId(),
            CurrencyId = request.CurrencyId
        };
        
        await handler.Handle(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("favorites")]
    [Authorize]
    [ProducesResponseType(typeof(GetAllCurrenciesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetFavorites([FromServices] GetUserFavoriteCurrenciesQueryHandler handler, CancellationToken cancellationToken)
    {
        var query = new GetUserFavoriteCurrenciesQuery
        {
            UserId = User.GetRequiredUserId()
        };

        var currencies = await handler.Handle(query, cancellationToken);
        
        // По хорошему. тут бы использовать либо автомаппер, либо написать свой маппер
        var currenciesDto = new GetAllCurrenciesResponse
        {
            Currencies = currencies.OrderBy(c => c.CharCode).Select(c => new CurrencyDto
            {
                Id = c.Id,
                CharCode = c.CharCode,
                Rate = c.Rate,
                UpdatedAt = c.UpdatedAt,

            }).ToList()
        };
        return Ok(currenciesDto);
    }

    [HttpDelete("favorites/{currencyId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveFavorite([FromRoute] Guid currencyId, [FromServices] RemoveFavoriteCurrencyCommandHandler handler, CancellationToken cancellationToken)
    {
        var command = new RemoveFavoriteCurrencyCommand
        {
            UserId = User.GetRequiredUserId(),
            CurrencyId = currencyId
        };
        
        await handler.Handle(command, cancellationToken);
        return NoContent();
    }
}