namespace UserService.Application.Exceptions;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string name)
        : base($"Пользователь {name} уже существует")
    { }
}