namespace FinanceService.Application.Commands;

/// <summary>
/// Команда для удаления валюты из избранного у пользователя
/// </summary>
public class RemoveFavoriteCurrencyCommand
{
    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; init; }
    
    /// <summary>
    /// ID валюты
    /// </summary>
    public Guid CurrencyId { get; init; }
}