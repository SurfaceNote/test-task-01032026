using CurrencyBackgroundService.Options;
using Microsoft.Extensions.Options;

namespace CurrencyBackgroundService.Services;

/// <summary>
/// Фоновый сервис для периодического обновления курсов валют
/// </summary>
public class CurrencyBackgroundWorker(
    CbrCurrencyFetcher cbrCurrencyFetcher,
    IServiceScopeFactory scopeFactory,
    ILogger<CurrencyBackgroundWorker> logger, 
    IOptions<CbrSettings> cbrOptions) 
    : BackgroundService
{
    private readonly TimeSpan _updateInterval =
        TimeSpan.FromMinutes(Math.Max(1, cbrOptions.Value.UpdateIntervalMinutes));
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Currency Background Worker запущен. Интервал обновления: {Interval} минут",
            _updateInterval.TotalMinutes);

        await UpdateCurrenciesAsync(cancellationToken);

        using var timer = new PeriodicTimer(_updateInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await UpdateCurrenciesAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Currency Background Worker останавливается");
        }
    }

    private async Task UpdateCurrenciesAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Запуск обновления курсов валют");

            var currenciesForCertainDay = await cbrCurrencyFetcher.FetchCurrenciesAsync(cancellationToken);
            
            await using var scope = scopeFactory.CreateAsyncScope();
            var currencyUpdateService = scope.ServiceProvider.GetRequiredService<CurrencyUpdateService>();
            
            await currencyUpdateService.UpdateCurrenciesAsync(currenciesForCertainDay, cancellationToken);
            
            logger.LogInformation(
                "Обновление курсов валют завершено успешно. Обновлено {Count} валют",
                currenciesForCertainDay.Currencies.Count);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обновлении курсов валют");
        }
    }
}