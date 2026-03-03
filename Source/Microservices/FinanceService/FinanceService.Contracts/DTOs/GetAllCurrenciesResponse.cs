namespace FinanceService.Contracts.DTOs;

/// <summary>
/// DTO со списком валют
/// </summary>
public class GetAllCurrenciesResponse
{
    /// <summary>
    /// Валюты
    /// </summary>
    public IReadOnlyCollection<CurrencyDto> Currencies { get; init; } = [];
}