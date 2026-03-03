using FluentValidation;

namespace FinanceService.Application.Commands.Validators;

/// <summary>
/// Валидатор для добавления валюты в избранное
/// </summary>
public class AddFavoriteCurrencyCommandValidator : AbstractValidator<AddFavoriteCurrencyCommand>
{
    public AddFavoriteCurrencyCommandValidator()
    {
        RuleFor(u => u.UserId)
            .NotEmpty()
            .WithMessage("Идентификатор пользователя обязателен");
        
        RuleFor(u => u.CurrencyId)
            .NotEmpty()
            .WithMessage("Идентификатор валюты обязателен");
    }
}