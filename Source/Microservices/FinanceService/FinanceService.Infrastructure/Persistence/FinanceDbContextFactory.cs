using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FinanceService.Infrastructure.Persistence;

public class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
{
    public FinanceDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        
        var configPath = Path.Combine(basePath, "appsettings.json");
        if (!File.Exists(configPath))
        {
            configPath = Path.Combine(basePath, "..", "..", "appsettings.json");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(configPath) ?? basePath)
            .AddJsonFile(Path.GetFileName(configPath), optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("FinanceDb") 
                               ?? "Host=localhost;Port=5432;Database=test-financedb;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new FinanceDbContext(optionsBuilder.Options);
    }
}