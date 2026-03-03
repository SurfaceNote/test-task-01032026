using FinanceService.Application.Repositories;
using FinanceService.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence.Repositories;

public class FavoriteCurrencyRepository(FinanceDbContext dbContext) : IFavoriteCurrencyRepository
{
    public async Task<bool> ExistsAsync(Guid userId, Guid currencyId, CancellationToken cancellationToken = default)
    {
        return await  dbContext.UserFavoriteCurrencies.AsNoTracking().AnyAsync(x => x.UserId == userId && x.CurrencyId == currencyId, cancellationToken);
    }

    public async Task AddAsync(UserFavoriteCurrency userFavoriteCurrency, CancellationToken cancellationToken = default)
    {
        await dbContext.UserFavoriteCurrencies.AddAsync(userFavoriteCurrency, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Currency>> GetFavoriteCurrenciesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserFavoriteCurrencies.AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Currency)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid currencyId, CancellationToken cancellationToken = default)
    {
        var existing =
            await dbContext.UserFavoriteCurrencies.FirstOrDefaultAsync(
                x => x.UserId == userId && x.CurrencyId == currencyId, cancellationToken);

        if (existing == null)
        {
            return false;
        }

        dbContext.UserFavoriteCurrencies.Remove(existing);
        return true;
    }
}