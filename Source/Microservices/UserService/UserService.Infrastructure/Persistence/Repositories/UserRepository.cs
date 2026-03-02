using Microsoft.EntityFrameworkCore;
using UserService.Application.Repository;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Repositories;

public class UserRepository(UserDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == userName, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Name == userName, cancellationToken);
    }
}