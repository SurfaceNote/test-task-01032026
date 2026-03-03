using FinanceService.Domain;

namespace FinanceService.Application.Repositories;

/// <summary>
/// Репозиторий для избранных валют
/// </summary>
public interface IFavoriteCurrencyRepository
{
    /// <summary>
    /// Проверяет, если пользователь уже добавил валюту в избранное
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="currencyId">ID валюты</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<bool> ExistsAsync(Guid userId, Guid currencyId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Добавляет валюту в избранное для пользователя
    /// </summary>
    /// <param name="userFavoriteCurrency">Валюта добавленная в избранное пользователем</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task AddAsync(UserFavoriteCurrency userFavoriteCurrency, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список валют, которые были добавлены пользователем в избранное
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<IReadOnlyCollection<Currency>> GetFavoriteCurrenciesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет валюту из избранного у пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="currencyId">ID валюты</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid userId, Guid currencyId, CancellationToken cancellationToken = default);
}