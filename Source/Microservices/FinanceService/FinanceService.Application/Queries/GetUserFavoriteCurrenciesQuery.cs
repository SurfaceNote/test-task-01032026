namespace FinanceService.Application.Queries;

/// <summary>
/// Запрос на получение валют, которые являются избранными для пользователя
/// </summary>
public class GetUserFavoriteCurrenciesQuery
{
    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; init; }
}