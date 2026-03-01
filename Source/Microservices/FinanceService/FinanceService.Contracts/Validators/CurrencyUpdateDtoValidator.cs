using FinanceService.Contracts.DTOs;
using FluentValidation;

namespace FinanceService.Contracts.Validators;

public class CurrencyUpdateDtoValidator : AbstractValidator<CurrencyUpdateDto>
{
    public CurrencyUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название валюты обязательно")
            .MaximumLength(255).WithMessage("Название валюты не должно превышать 255 символов");
        
        RuleFor(x => x.CharCode)
            .NotEmpty().WithMessage("Код валюты обязателен")
            .Length(3).WithMessage("Код валюты должен содержать 3 символа")
            .Matches("^[A-Z]{3}$").WithMessage("Код валюты должен состоять из 3 заглавных латинских букв");
        
        RuleFor(x => x.Rate)
            .GreaterThan(0).WithMessage("Курс валюты должен быть больше нуля");
    }
}