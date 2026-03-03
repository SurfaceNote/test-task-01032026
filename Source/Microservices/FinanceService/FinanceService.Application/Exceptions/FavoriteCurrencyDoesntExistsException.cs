namespace FinanceService.Application.Exceptions;

public sealed class FavoriteCurrencyDoesntExistsException(Guid userId, Guid currencyId)
    : Exception($"Валюта {currencyId} не была добавлена в избранное пользователя {userId}")
{
}