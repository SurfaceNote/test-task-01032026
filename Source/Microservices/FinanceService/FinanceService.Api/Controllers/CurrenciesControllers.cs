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
}