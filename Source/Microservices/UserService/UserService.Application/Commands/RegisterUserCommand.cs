namespace UserService.Application.Commands;

/// <summary>
/// Команда для регистрации пользователя
/// </summary>
public class RegisterUserCommand
{
    public string Name { get; init; } = null!;
    public string Password { get; init; } = null!;
}