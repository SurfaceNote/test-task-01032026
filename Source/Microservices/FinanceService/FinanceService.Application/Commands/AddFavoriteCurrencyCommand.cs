namespace FinanceService.Application.Commands;

/// <summary>
/// Команда для добавления валюты в избраное для пользователя
/// </summary>
public class AddFavoriteCurrencyCommand
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