using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Infrastructure.Persistence;

namespace MigrationService;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Console.WriteLine("=== EF Core Migration Service ===\n");

        // Читаем конфигурацию
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        // Получаем аргумент --database (по умолчанию "all")
        var database = configuration["database"] ?? "all";
        
        // Показываем справку
        if (args.Contains("--help") || args.Contains("-h"))
        {
            ShowHelp();
            return 0;
        }

        Console.WriteLine($"Target database: {database}\n");

        try
        {
            var services = ConfigureServices(configuration);
            using var scope = services.CreateScope();

            // Применяем миграции для указанной БД
            if (database == "all" || database == "userdb")
            {
                await ApplyMigrations<UserDbContext>(scope.ServiceProvider, "UserDb");
            }
            
            if (database == "all" || database == "financedb")
            {
                 await ApplyMigrations<FinanceDbContext>(scope.ServiceProvider, "FinanceDb");
            }

            Console.WriteLine("\nAll migrations completed successfully!\n");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nMigration failed: {ex.Message}\n");
            Console.WriteLine(ex.StackTrace);
            return 1;
        }
    }

    private static async Task ApplyMigrations<TContext>(IServiceProvider serviceProvider, string dbName) 
        where TContext : DbContext
    {
        Console.WriteLine($"--- {dbName} ---");
        
        var context = serviceProvider.GetRequiredService<TContext>();
        var pending = await context.Database.GetPendingMigrationsAsync();

        if (!pending.Any())
        {
            Console.WriteLine("No pending migrations\n");
            return;
        }

        Console.WriteLine($"Applying {pending.Count()} migration(s)...");
        await context.Database.MigrateAsync();
        Console.WriteLine("✓ Done\n");
    }

    private static ServiceProvider ConfigureServices(IConfiguration configuration)
    {
        var services = new ServiceCollection();

        // UserDb
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("UserDb"),
                b => b.MigrationsAssembly("UserService.Infrastructure")));

        // FinanceDb
         services.AddDbContext<FinanceDbContext>(options =>
             options.UseNpgsql(
                 configuration.GetConnectionString("FinanceDb"),
                 b => b.MigrationsAssembly("FinanceService.Infrastructure")));

        return services.BuildServiceProvider();
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run                           - Apply migrations to all databases");
        Console.WriteLine("  dotnet run --database userdb         - Apply migrations to UserDb only");
        Console.WriteLine("  dotnet run --database financedb      - Apply migrations to FinanceDb only");
        Console.WriteLine("  dotnet run --help                    - Show this help");
        Console.WriteLine();
    }
}