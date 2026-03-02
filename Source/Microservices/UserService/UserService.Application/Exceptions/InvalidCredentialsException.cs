namespace UserService.Application.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() 
        : base("Неправильные логин или пароль")
    {
    }
}