namespace UserService.Application.Exceptions;

public class UserAlreadyExistsException(string name) : Exception($"Пользователь {name} уже существует");