using Common.Persistence;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using UserService.Application.Commands;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Application.Repository;
using UserService.Domain.Entities;

namespace UserService.Tests;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IValidator<RegisterUserCommand>> _validator = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenService> _jwtTokenService = new();

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        var command = new RegisterUserCommand { Name = "name", Password = "password" };
        var validationErrors = new[] { new ValidationFailure("Name", "Invalid name") };
        _validator.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationErrors));

        var sut = CreateSut();

        await Assert.ThrowsAsync<ValidationException>(() => sut.Handle(command, CancellationToken.None));

        _userRepository.Verify(x => x.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowUserAlreadyExistsException_WhenUserAlreadyExists()
    {
        var command = new RegisterUserCommand { Name = "  existing-user  ", Password = "password" };
        _validator.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _userRepository.Setup(x => x.ExistsByNameAsync("existing-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = CreateSut();

        await Assert.ThrowsAsync<UserAlreadyExistsException>(() => sut.Handle(command, CancellationToken.None));

        _userRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserAndReturnToken_WhenCommandIsValid()
    {
        var command = new RegisterUserCommand { Name = "  test-user  ", Password = "password" };
        User? addedUser = null;

        _validator.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _userRepository.Setup(x => x.ExistsByNameAsync("test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _passwordHasher.Setup(x => x.HashPassword("password")).Returns("hashed-password");
        _userRepository.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => addedUser = user)
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _jwtTokenService.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("token");

        var sut = CreateSut();

        var result = await sut.Handle(command, CancellationToken.None);

        Assert.Equal("token", result);
        Assert.NotNull(addedUser);
        Assert.Equal("test-user", addedUser!.Name);
        Assert.Equal("hashed-password", addedUser.PasswordHash);
        _userRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _jwtTokenService.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Once);
    }

    private RegisterUserCommandHandler CreateSut()
    {
        return new RegisterUserCommandHandler(
            _userRepository.Object,
            _validator.Object,
            _unitOfWork.Object,
            _passwordHasher.Object,
            _jwtTokenService.Object);
    }
}
