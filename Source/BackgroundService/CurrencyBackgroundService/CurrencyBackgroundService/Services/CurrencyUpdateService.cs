using CurrencyBackgroundService.Models;
using FinanceService.Contracts.DTOs;
using FinanceService.Contracts.Validators;
using FinanceService.Domain;
using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CurrencyBackgroundService.Services;

/// <summary>
/// Сервис для обновления валют в базе данных
/// </summary>
public class CurrencyUpdateService(
    FinanceDbContext dbContext,
    ILogger<CurrencyUpdateService> logger,
    CurrencyUpdateDtoValidator validator)
{
    public async Task UpdateCurrenciesAsync(CbrCurrenciesForCertainDay currenciesForCertainDay,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Начинается обновление {Count} валют в БД", currenciesForCertainDay.Currencies.Count);

            var successCount = 0;
            var errorCount = 0;

            foreach (var cbrCurrency in currenciesForCertainDay.Currencies)
            {
                try
                {
                    // Преобразуем в DTO для обновления валюты
                    var currencyDto = new CurrencyUpdateDto
                    {
                        Name = cbrCurrency.Name,
                        CharCode = cbrCurrency.CharCode,
                        Rate = cbrCurrency.VunitRate
                    };
                    
                    // Валидация
                    var validationResult = await validator.ValidateAsync(currencyDto, cancellationToken);

                    if (!validationResult.IsValid)
                    {
                        logger.LogWarning("Валюта {CharCode} не прошла валидацию: {Errors}",
                            cbrCurrency.CharCode,
                            string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                        errorCount++;
                        continue;
                    }

                    await UpsertCurrencyAsync(currencyDto, currenciesForCertainDay.Date, cancellationToken);
                    successCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при обновлении валюты {CharCode}", cbrCurrency.CharCode);
                    errorCount++;
                }
            }
            
            logger.LogInformation("Обновление завершено. Успешно: {SuccessCount}, Ошибок: {ErrorCount}", 
                successCount, errorCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Критическая ошибка при обновлении валют");
            throw;
        }
    }

    /// <summary>
    /// Создаем или обновляем валюту в БД
    /// </summary>
    private async Task UpsertCurrencyAsync(CurrencyUpdateDto currency, DateOnly latestInfoDate, CancellationToken cancellationToken)
    {
        // Ищем существующую валюту по CharCode
        var existingCurrency =
            await dbContext.Currencies.FirstOrDefaultAsync(c => c.CharCode == currency.CharCode, cancellationToken);
        
        if (existingCurrency is not null)
        {
            // Обновляем существующую валюту
            existingCurrency.SetNewRate(currency.Rate);
        }
        else
        {
            // Создаем новую запись о валюте
            var newCurrency = new Currency(currency.Name, currency.CharCode, currency.Rate);
            
            await dbContext.Currencies.AddAsync(newCurrency, cancellationToken);
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}