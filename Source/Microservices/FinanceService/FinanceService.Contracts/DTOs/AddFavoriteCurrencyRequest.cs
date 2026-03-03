namespace FinanceService.Contracts.DTOs;

/// <summary>
/// Запрос на добавление валюты в избранное
/// </summary>
public class AddFavoriteCurrencyRequest
{
    /// <summary>
    /// ID валюты
    /// </summary>
    public Guid CurrencyId { get; init; }
}