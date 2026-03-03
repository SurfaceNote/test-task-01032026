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
    Task<IReadOnlyCollection<Currency>> GetAllCurrencies(CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, если валюта существует с заданным ID
    /// </summary>
    /// <param name="currencyId">ID валюты</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<bool> ExistsByIdAsync(Guid currencyId, CancellationToken cancellationToken = default);
}