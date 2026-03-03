using FinanceService.Application.Repositories;
using FinanceService.Domain;

namespace FinanceService.Application.Queries;

public class GetUserFavoriteCurrenciesQueryHandler(IFavoriteCurrencyRepository favoriteCurrencyRepository)
{
    public async Task<IReadOnlyCollection<Currency>> Handle(GetUserFavoriteCurrenciesQuery query,
        CancellationToken cancellationToken = default)
    {
        var currencies = await favoriteCurrencyRepository.GetFavoriteCurrenciesAsync(query.UserId, cancellationToken);
        
        return currencies;
    }
}