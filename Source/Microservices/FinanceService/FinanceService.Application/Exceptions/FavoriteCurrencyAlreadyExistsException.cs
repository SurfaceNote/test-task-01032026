namespace FinanceService.Application.Exceptions;

public sealed class FavoriteCurrencyAlreadyExistsException(Guid userId, Guid currencyId) 
    : Exception($"Валюта {currencyId} уже добавлена в избранное пользователя {userId}")
{
}