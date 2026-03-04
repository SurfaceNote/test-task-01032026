using FinanceService.Application.Queries;
using FinanceService.Application.Repositories;
using FinanceService.Domain;
using Moq;

namespace FinanceService.Tests;

public class CurrencyQueryHandlersTests
{
    [Fact]
    public async Task GetAllCurrencies_Handle_ShouldReturnCurrenciesFromRepository()
    {
        var expected = new List<Currency> { new("US Dollar", "USD", 90.5m), new("Euro", "EUR", 99.9m) };
        var repository = new Mock<ICurrencyRepository>();
        repository.Setup(x => x.GetAllCurrencies(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var sut = new GetAllCurrenciesQueryHandler(repository.Object);

        var result = await sut.Handle(new GetAllCurrenciesQuery(), CancellationToken.None);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task GetUserFavoriteCurrencies_Handle_ShouldReturnFavoritesFromRepository()
    {
        var expected = new List<Currency> { new("US Dollar", "USD", 90.5m) };
        var repository = new Mock<IFavoriteCurrencyRepository>();
        var userId = Guid.NewGuid();
        repository.Setup(x => x.GetFavoriteCurrenciesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var sut = new GetUserFavoriteCurrenciesQueryHandler(repository.Object);

        var result = await sut.Handle(new GetUserFavoriteCurrenciesQuery { UserId = userId }, CancellationToken.None);

        Assert.Same(expected, result);
    }
}
