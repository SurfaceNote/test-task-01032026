namespace FinanceService.Contracts.DTOs;

/// <summary>
/// DTO для обновления валюты
/// </summary>
public class CurrencyUpdateDto
{
    /// <summary>
    /// Название валюты
    /// </summary>
    public string Name { get; init; } = null!;
    
    /// <summary>
    /// ISO Букв. код валюты
    /// </summary>
    public string CharCode { get; init; } = null!;
    
    /// <summary>
    /// Курс за 1 единицу валюты
    /// </summary>
    public decimal Rate { get; init; }
}