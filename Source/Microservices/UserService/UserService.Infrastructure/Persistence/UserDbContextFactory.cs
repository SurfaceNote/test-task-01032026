using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace UserService.Infrastructure.Persistence;

public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        // Ищем appsettings.json в Infrastructure проекте или в родительских папках
        var basePath = Directory.GetCurrentDirectory();
        
        // Если запускаем из MigrationService, ищем конфиг там
        var configPath = Path.Combine(basePath, "appsettings.json");
        if (!File.Exists(configPath))
        {
            // Попробуем найти в корне решения
            configPath = Path.Combine(basePath, "..", "..", "appsettings.json");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(configPath) ?? basePath)
            .AddJsonFile(Path.GetFileName(configPath), optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("UserDb") 
                               ?? "Host=localhost;Port=5432;Database=test-userdb;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new UserDbContext(optionsBuilder.Options);
    }
}