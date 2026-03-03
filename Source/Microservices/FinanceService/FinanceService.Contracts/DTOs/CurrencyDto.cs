namespace FinanceService.Contracts.DTOs;

/// <summary>
/// DTO валюты
/// </summary>
public class CurrencyDto
{
    public Guid Id { get; init; }
    
    /// <summary>
    /// ISO Букв. код валюты
    /// </summary>
    public string CharCode { get; init; } = null!;
    
    /// <summary>
    /// Курс за 1 единицу валюты
    /// </summary>
    public decimal Rate { get; init; }
    
    public DateTime UpdatedAt { get; init; }
}