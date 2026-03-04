using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Api.Init;

public static class DatabaseInitExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("FinanceDb");

        services.AddDbContext<FinanceDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
