using FluentValidation;

namespace UserService.Application.Commands.Validators;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Имя должно быть заполнено")
            .Must(name => name is not null && !name.Any(char.IsWhiteSpace))
            .WithMessage("Имя не должно содержать пробелы")
            .Matches(@"^[\p{L}\p{Nd}]+$")
            .WithMessage("Имя может содержать только буквы и цифры");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Пароль должен быть заполнен")
            .Must(password => password is not null && !password.Any(char.IsWhiteSpace))
            .WithMessage("Пароль не должен содержать пробелы");
    }
}