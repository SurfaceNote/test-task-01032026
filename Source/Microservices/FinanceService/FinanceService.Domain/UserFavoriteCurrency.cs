namespace FinanceService.Domain;

/// <summary>
/// Записи о любимых валютах пользователя
/// </summary>
public class UserFavoriteCurrency
{
    public Guid Id { get; private set; }
    
    /// <summary>
    /// ИД Пользователя
    /// </summary>
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// ИД Валюты
    /// </summary>
    public Guid CurrencyId { get; private set; }
    
    // Навигационное свойство 
    public Currency Currency { get; private set; } = null!;
}