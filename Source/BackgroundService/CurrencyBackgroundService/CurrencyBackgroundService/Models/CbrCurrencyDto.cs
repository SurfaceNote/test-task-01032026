namespace CurrencyBackgroundService.Models;

/// <summary>
/// DTO для данных одной валюты из API ЦБ РФ
/// </summary>
public class CbrCurrencyDto
{
    /// <summary>
    /// Числовой код валюты по ISO 4217 (например, 941)
    /// </summary>
    public string NumCode { get; set; } = null!;

    /// <summary>
    /// Буквенный код валюты по ISO 4217 (например, RSD)
    /// </summary>
    public string CharCode { get; set; } = null!;

    /// <summary>
    /// Номинал валюты, для которого указан курс (например, 100)
    /// </summary>
    public int Nominal { get; set; }

    /// <summary>
    /// Наименование валюты (например, "Сербских динаров")
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Курс указанного номинала валюты к рублю (например, 77.7335 за 100 RSD)
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Курс одной единицы валюты к рублю (например, 0.777335 за 1 RSD)
    /// </summary>
    public decimal VunitRate { get; set; }
}