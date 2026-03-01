using System.Text;
using CurrencyBackgroundService.Options;
using CurrencyBackgroundService.Services;
using FinanceService.Contracts.Validators;
using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Polly;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("FinanceDb");

builder.Services.AddDbContext<FinanceDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services
    .AddHttpClient<CbrCurrencyFetcher>(client =>
    {
        client.BaseAddress = new Uri("https://www.cbr.ru/");
        client.Timeout = TimeSpan.FromSeconds(15);
    })
    .AddResilienceHandler("cbr-pipeline", (pipeline, context) =>
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<CbrCurrencyFetcher>>();

        pipeline.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromSeconds(2),
            UseJitter = false,

            OnRetry = args =>
            {
                logger.LogWarning(
                    "CBR retry #{RetryAttempt}. Повтор через {Delay}с. StatusCode: {StatusCode}. Exception: {ExceptionType}",
                    args.AttemptNumber + 1,
                    args.RetryDelay.TotalSeconds,
                    args.Outcome.Result?.StatusCode,
                    args.Outcome.Exception?.GetType().Name);

                return default;
            }
        });
    });

builder.Services.AddHostedService<CurrencyBackgroundWorker>();
builder.Services.AddScoped<CurrencyUpdateService>();

builder.Services.AddScoped<CurrencyUpdateDtoValidator>();

builder.Services.Configure<CbrSettings>(
    builder.Configuration.GetSection("CbrSettings"));

var host = builder.Build();
host.Run();