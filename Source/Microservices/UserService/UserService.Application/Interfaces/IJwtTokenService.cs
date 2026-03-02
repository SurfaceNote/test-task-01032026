using UserService.Domain.Entities;

namespace UserService.Application.Interfaces;

/// <summary>
/// Сервис для работы с JWT токеном
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Создает токен
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <returns>Токен и доп.инфу</returns>
    string CreateToken(User user);
}