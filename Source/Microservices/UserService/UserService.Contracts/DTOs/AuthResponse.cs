namespace UserService.Contracts.DTOs;

/// <summary>
/// Ответ на авторизацию
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// JWT токен
    /// </summary>
    public string AccessToken { get; init; } = null!;
}