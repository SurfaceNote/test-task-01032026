using Common.Persistence;
using FinanceService.Application.Commands;
using FinanceService.Application.Commands.Validators;
using FinanceService.Application.Queries;
using FinanceService.Application.Repositories;
using FinanceService.Infrastructure.Persistence;
using FinanceService.Infrastructure.Persistence.Repositories;
using FluentValidation;

namespace FinanceService.Api.Init;

public static class ApplicationInitExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IFavoriteCurrencyRepository, FavoriteCurrencyRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork<FinanceDbContext>>();

        services.AddScoped<GetAllCurrenciesQueryHandler>();
        services.AddScoped<AddFavoriteCurrencyCommandHandler>();
        services.AddScoped<GetUserFavoriteCurrenciesQueryHandler>();
        services.AddScoped<RemoveFavoriteCurrencyCommandHandler>();

        services.AddValidatorsFromAssemblyContaining<AddFavoriteCurrencyCommandValidator>();

        return services;
    }
}
