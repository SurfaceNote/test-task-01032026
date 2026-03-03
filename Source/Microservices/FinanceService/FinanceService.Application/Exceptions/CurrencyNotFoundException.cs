namespace FinanceService.Application.Exceptions;

public sealed class CurrencyNotFoundException(Guid currencyId) : Exception($"Валюта с id {currencyId} не найдена")
{
}