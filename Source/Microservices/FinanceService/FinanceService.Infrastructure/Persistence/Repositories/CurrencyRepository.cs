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

    public async Task<bool> ExistsByIdAsync(Guid currencyId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Currencies.AsNoTracking().AnyAsync(c => c.Id == currencyId, cancellationToken);
    }
}