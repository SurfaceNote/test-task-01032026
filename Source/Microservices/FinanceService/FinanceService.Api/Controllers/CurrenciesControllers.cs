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
}