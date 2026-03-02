namespace UserService.Application.Interfaces;

/// <summary>
/// Сервис хэширования пароля
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Хэширование пароля
    /// </summary>
    /// <param name="password">Пароль</param>
    /// <returns>Захешированный пароль</returns>
    string HashPassword(string password);

    /// <summary>
    /// Проверяет пароль с хэшом
    /// </summary>
    /// <param name="password">Пароль</param>
    /// <param name="hashedPassword">Хэш пароля</param>
    /// <returns></returns>
    bool VerifyHashedPassword(string password, string hashedPassword);
}