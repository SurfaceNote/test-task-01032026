using UserService.Domain.Entities;

namespace UserService.Application.Repository;

/// <summary>
/// Репозиторий для работы с пользователем
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Добавить пользователя
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить пользователя по нику
    /// </summary>
    /// <param name="userName">ник пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь</returns>
    Task<User?> GetByNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет существует ли пользователь с таким ником
    /// </summary>
    /// <param name="userName">ник пользователя</param>
    /// <param name="cancellationToken">токен отмены</param>
    /// <returns></returns>
    Task<bool> ExistsByNameAsync(string userName, CancellationToken cancellationToken = default);
}