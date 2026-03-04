using Common.Persistence;
using FinanceService.Application.Commands;
using FinanceService.Application.Exceptions;
using FinanceService.Application.Repositories;
using FinanceService.Domain;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FinanceService.Tests;

public class AddFavoriteCurrencyCommandHandlerTests
{
    private readonly Mock<IFavoriteCurrencyRepository> _favoriteCurrencyRepository = new();
    private readonly Mock<ICurrencyRepository> _currencyRepository = new();
    private readonly Mock<IValidator<AddFavoriteCurrencyCommand>> _validator = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        var command = new AddFavoriteCurrencyCommand { UserId = Guid.NewGuid(), CurrencyId = Guid.NewGuid() };
        var validationErrors = new[] { new ValidationFailure("CurrencyId", "Invalid currency id") };

        _validator.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationErrors));

        var sut = CreateSut();

        await Assert.ThrowsAsync<ValidationException>(() => sut.Handle(command, CancellationToken.None));

        _currencyRepository.Verify(x => x.ExistsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _favoriteCurrencyRepository.Verify(x => x.AddAsync(It.IsAny<UserFavoriteCurrency>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowCurrencyNotFoundException_WhenCurrencyDoesNotExist()
    {
        var command = new AddFavoriteCurrencyCommand { UserId = Guid.NewGuid(), CurrencyId = Guid.NewGuid() };
        _validator.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _currencyRepository.Setup(x => x.ExistsByIdAsync(command.CurrencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = CreateSut();

        await Assert.ThrowsAsync<CurrencyNotFoundException>(() => sut.Handle(command, CancellationToken.None));

        _favoriteCurrencyRepository.Verify(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowFavoriteCurrencyAlreadyExistsException_WhenFavoriteAlreadyExists()
    {
        var command = new AddFavoriteCurrencyCommand { UserId = Guid.NewGuid(), CurrencyId = Guid.NewGuid() };
        _validator.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _currencyRepository.Setup(x => x.ExistsByIdAsync(command.CurrencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _favoriteCurrencyRepository.Setup(x =>
                x.ExistsAsync(command.UserId, command.CurrencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = CreateSut();

        await Assert.ThrowsAsync<FavoriteCurrencyAlreadyExistsException>(() => sut.Handle(command, CancellationToken.None));

        _favoriteCurrencyRepository.Verify(x => x.AddAsync(It.IsAny<UserFavoriteCurrency>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldAddFavoriteAndSave_WhenCommandIsValid()
    {
        var command = new AddFavoriteCurrencyCommand { UserId = Guid.NewGuid(), CurrencyId = Guid.NewGuid() };
        UserFavoriteCurrency? addedFavorite = null;

        _validator.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _currencyRepository.Setup(x => x.ExistsByIdAsync(command.CurrencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _favoriteCurrencyRepository.Setup(x =>
                x.ExistsAsync(command.UserId, command.CurrencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _favoriteCurrencyRepository.Setup(x => x.AddAsync(It.IsAny<UserFavoriteCurrency>(), It.IsAny<CancellationToken>()))
            .Callback<UserFavoriteCurrency, CancellationToken>((favorite, _) => addedFavorite = favorite)
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var sut = CreateSut();

        await sut.Handle(command, CancellationToken.None);

        Assert.NotNull(addedFavorite);
        Assert.Equal(command.UserId, addedFavorite!.UserId);
        Assert.Equal(command.CurrencyId, addedFavorite.CurrencyId);
        _favoriteCurrencyRepository.Verify(x => x.AddAsync(It.IsAny<UserFavoriteCurrency>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private AddFavoriteCurrencyCommandHandler CreateSut()
    {
        return new AddFavoriteCurrencyCommandHandler(
            _favoriteCurrencyRepository.Object,
            _currencyRepository.Object,
            _validator.Object,
            _unitOfWork.Object);
    }
}
