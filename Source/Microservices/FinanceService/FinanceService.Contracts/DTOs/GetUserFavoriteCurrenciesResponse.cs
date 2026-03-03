namespace FinanceService.Contracts.DTOs;

/// <summary>
/// DTO со списком избранной валюты для пользователя
/// </summary>
public class GetUserFavoriteCurrenciesResponse
{
    /// <summary>
    /// Валюты
    /// </summary>
    public IReadOnlyCollection<CurrencyDto> Currencies { get; init; } = [];
}