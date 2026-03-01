namespace CurrencyBackgroundService.Models;

/// <summary>
/// DTO для данных валют из API ЦБ РФ
/// </summary>
public class CbrCurrenciesForCertainDay
{
    /// <summary>
    /// За какую дату получили ифнормацию по валюте
    /// </summary>
    public DateOnly Date { get; init; }
    
    /// <summary>
    /// Список валют
    /// </summary>
    public List<CbrCurrencyDto> Currencies { get; init; } = [];
}