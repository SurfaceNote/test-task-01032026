using Moq;
using UserService.Application.Commands;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Application.Repository;
using UserService.Domain.Entities;

namespace UserService.Tests;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenService> _jwtTokenService = new();

    [Fact]
    public async Task Handle_ShouldThrowInvalidCredentialsException_WhenUserNotFound()
    {
        var command = new LoginUserCommand { Name = "  missing-user  ", Password = "password" };
        _userRepository.Setup(x => x.GetByNameAsync("missing-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = CreateSut();

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => sut.Handle(command, CancellationToken.None));

        _jwtTokenService.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidCredentialsException_WhenPasswordIsInvalid()
    {
        var command = new LoginUserCommand { Name = "user", Password = "wrong-password" };
        var user = new User("user", "hashed-password");

        _userRepository.Setup(x => x.GetByNameAsync("user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(x => x.VerifyHashedPassword("wrong-password", "hashed-password")).Returns(false);

        var sut = CreateSut();

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => sut.Handle(command, CancellationToken.None));

        _jwtTokenService.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var command = new LoginUserCommand { Name = "  user  ", Password = "password" };
        var user = new User("user", "hashed-password");

        _userRepository.Setup(x => x.GetByNameAsync("user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(x => x.VerifyHashedPassword("password", "hashed-password")).Returns(true);
        _jwtTokenService.Setup(x => x.CreateToken(user)).Returns("token");

        var sut = CreateSut();

        var result = await sut.Handle(command, CancellationToken.None);

        Assert.Equal("token", result);
        _jwtTokenService.Verify(x => x.CreateToken(user), Times.Once);
    }

    private LoginUserCommandHandler CreateSut()
    {
        return new LoginUserCommandHandler(_userRepository.Object, _passwordHasher.Object, _jwtTokenService.Object);
    }
}
