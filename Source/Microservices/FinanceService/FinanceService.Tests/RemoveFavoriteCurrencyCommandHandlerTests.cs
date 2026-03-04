using Common.Persistence;
using FinanceService.Application.Commands;
using FinanceService.Application.Exceptions;
using FinanceService.Application.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FinanceService.Tests;

public class RemoveFavoriteCurrencyCommandHandlerTests
{
    private readonly Mock<IFavoriteCurrencyRepository> _favoriteCurrencyRepository = new();
    private readonly Mock<IValidator<RemoveFavoriteCurrencyCommand>> _validator = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        var command = new RemoveFavoriteCurrencyCommand { UserId = Guid.NewGuid(), CurrencyId = Guid.NewGuid() };
        var validationErrors = new[] { new ValidationFailure("CurrencyId", "Invalid currency id") };
        _validator.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationErrors));

        var sut = CreateSut();

        await Assert.ThrowsAsync<ValidationException>(() => sut.Handle(command, CancellationToken.None));

        _favoriteCurrencyRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowFavoriteCurrencyDoesntExistsException_WhenDeleteReturnsFalse()
    {
        var command = new RemoveFavoriteCurrencyCommand { UserId = Guid.NewGuid(), CurrencyId = Guid.NewGuid() };
        _validator.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _favoriteCurrencyRepository
            .Setup(x => x.DeleteAsync(command.UserId, command.CurrencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = CreateSut();

        await Assert.ThrowsAsync<FavoriteCurrencyDoesntExistsException>(() => sut.Handle(command, CancellationToken.None));

        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSaveChanges_WhenDeleteReturnsTrue()
    {
        var command = new RemoveFavoriteCurrencyCommand { UserId = Guid.NewGuid(), CurrencyId = Guid.NewGuid() };
        _validator.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _favoriteCurrencyRepository
            .Setup(x => x.DeleteAsync(command.UserId, command.CurrencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var sut = CreateSut();

        await sut.Handle(command, CancellationToken.None);

        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private RemoveFavoriteCurrencyCommandHandler CreateSut()
    {
        return new RemoveFavoriteCurrencyCommandHandler(
            _favoriteCurrencyRepository.Object,
            _validator.Object,
            _unitOfWork.Object);
    }
}
