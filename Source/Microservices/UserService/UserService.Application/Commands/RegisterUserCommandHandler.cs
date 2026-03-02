using Common.Persistence;
using FluentValidation;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Application.Repository;
using UserService.Domain.Entities;

namespace UserService.Application.Commands;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IValidator<RegisterUserCommand> commandValidator,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher, 
    IJwtTokenService jwtTokenService)
{
    public async Task<string> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await commandValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var normalizedName = command.Name.Trim();

        if (await userRepository.ExistsByNameAsync(normalizedName, cancellationToken))
        {
            throw new UserAlreadyExistsException(normalizedName);
        }

        var passwordHash = passwordHasher.HashPassword(command.Password);
        var user = new User(normalizedName, passwordHash);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return jwtTokenService.CreateToken(user);
    }
    
}