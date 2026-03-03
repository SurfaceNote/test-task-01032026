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
}