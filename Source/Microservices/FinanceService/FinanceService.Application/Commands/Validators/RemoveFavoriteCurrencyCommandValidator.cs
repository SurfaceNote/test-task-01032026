using FluentValidation;

namespace FinanceService.Application.Commands.Validators;

/// <summary>
/// Валидатор для удаления валюты из избранного у пользователя
/// </summary>
public class RemoveFavoriteCurrencyCommandValidator : AbstractValidator<RemoveFavoriteCurrencyCommand>
{
    public RemoveFavoriteCurrencyCommandValidator()
    {
        RuleFor(u => u.UserId)
            .NotEmpty()
            .WithMessage("Идентификатор пользователя обязателен");
        
        RuleFor(u => u.CurrencyId)
            .NotEmpty()
            .WithMessage("Идентификатор валюты обязателен");
    }
}