using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.Persistence;

namespace UserService.Api.Init;

public static class DatabaseInitExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("UserDb");

        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
