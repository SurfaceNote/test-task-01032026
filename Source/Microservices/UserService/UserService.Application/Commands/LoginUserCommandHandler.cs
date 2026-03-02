using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Application.Repository;

namespace UserService.Application.Commands;

public class LoginUserCommandHandler(
    IUserRepository userRepository, 
    IPasswordHasher passwordHasher, 
    IJwtTokenService jwtTokenService)
{
    public async Task<string> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        // Примечание, эта команда не будет делать никаких обновлений или новых записей. Поэтому надо бы убрать его в Query.
        // Но на боевом проекте мы бы добавляли RefreshToken, поэтому оставил в Command

        var user = await userRepository.GetByNameAsync(command.Name.Trim(), cancellationToken);

        if (user is null || !passwordHasher.VerifyHashedPassword(command.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        return jwtTokenService.CreateToken(user);
    }
}