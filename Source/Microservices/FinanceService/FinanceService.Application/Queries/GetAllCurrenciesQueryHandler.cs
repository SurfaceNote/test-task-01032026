using FinanceService.Application.Repositories;
using FinanceService.Domain;

namespace FinanceService.Application.Queries;

public class GetAllCurrenciesQueryHandler(ICurrencyRepository currencyRepository)
{
    public async Task<IReadOnlyCollection<Currency>> Handle(GetAllCurrenciesQuery query,
        CancellationToken cancellationToken)
    {
        var currencies = await currencyRepository.GetAllCurrencies(cancellationToken);

        return currencies;
    }
}