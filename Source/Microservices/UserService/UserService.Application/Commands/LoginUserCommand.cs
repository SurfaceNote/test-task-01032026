namespace UserService.Application.Commands;

/// <summary>
/// Команда для авторизации
/// </summary>
public class LoginUserCommand
{
    // Примечание, эта команда не будет делать никаких обновлений или новых записей. Поэтому надо бы убрать его в Query.
    // Но на боевом проекте мы бы добавляли RefreshToken, поэтому оставил в Command
    
    public string Name { get; init; } = null!;
    public string Password { get; init; } = null!;
}