using Microsoft.EntityFrameworkCore;

namespace Common.Persistence;

public class UnitOfWork<TDbContext>(TDbContext dbContext) : IUnitOfWork
    where TDbContext : DbContext
{
    private TDbContext Context => dbContext;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }

}