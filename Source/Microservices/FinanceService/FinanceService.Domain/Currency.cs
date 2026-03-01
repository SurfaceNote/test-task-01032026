namespace FinanceService.Domain;

/// <summary>
/// Валюта
/// </summary>
public class Currency
{
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Название валюты
    /// </summary>
    public string Name { get; private set; } = null!;
    
    /// <summary>
    /// ISO Букв. код валюты
    /// </summary>
    public string CharCode { get; private set; } = null!;
    
    /// <summary>
    /// Курс за 1 единицу валюты
    /// </summary>
    public decimal Rate { get; private set; }
    
    public DateTime UpdatedAt { get; private set; }
    
    // Навигационное свойство
    public List<UserFavoriteCurrency> FavoriteByUsers { get; set; } = [];

    public Currency(string name, string charCode, decimal rate)
    {
        Id = Guid.NewGuid();
        Name = name;
        CharCode = charCode;
        Rate = rate;
    }
    
    /// <summary>
    /// Устанавливаем новый курс для валюты
    /// </summary>
    /// <param name="rate">Новый курс</param>
    public void SetNewRate(decimal rate)
    {
        Rate = rate;
        UpdatedAt = DateTime.UtcNow;
    }
}