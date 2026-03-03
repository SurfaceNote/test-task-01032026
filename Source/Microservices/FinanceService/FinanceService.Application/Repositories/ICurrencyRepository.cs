using FinanceService.Domain;

namespace FinanceService.Application.Repositories;

/// <summary>
/// Репозиторий для валют
/// </summary>
public interface ICurrencyRepository
{
    /// <summary>
    /// Получить все валюты
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список валют</returns>
    Task<IReadOnlyCollection<Currency>> GetAllCurrencies(CancellationToken cancellationToken);
}