using FinanceService.Application.Repositories;
using FinanceService.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence.Repositories;

public class CurrencyRepository(FinanceDbContext dbContext) : ICurrencyRepository
{
    public async Task<IReadOnlyCollection<Currency>> GetAllCurrencies(CancellationToken cancellationToken)
    {
        return await dbContext.Currencies.AsNoTracking().ToListAsync(cancellationToken);
    }
}