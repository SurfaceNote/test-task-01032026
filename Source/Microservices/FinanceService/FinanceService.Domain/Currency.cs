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
    /// Курс за 1 рубль
    /// </summary>
    public decimal Rate { get; private set; }
    
    public DateTime UpdatedAt { get; private set; }
    
    // Навигационное свойство
    public List<UserFavoriteCurrency> FavoriteByUsers { get; set; } = [];
}