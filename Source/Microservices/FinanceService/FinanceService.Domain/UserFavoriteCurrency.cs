namespace FinanceService.Domain;

/// <summary>
/// Записи о любимых валютах пользователя
/// </summary>
public class UserFavoriteCurrency(Guid userId, Guid currencyId)
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// ИД Пользователя
    /// </summary>
    public Guid UserId { get; private set; } = userId;

    /// <summary>
    /// ИД Валюты
    /// </summary>
    public Guid CurrencyId { get; private set; } = currencyId;

    // Навигационное свойство 
    public Currency Currency { get; private set; } = null!;
}