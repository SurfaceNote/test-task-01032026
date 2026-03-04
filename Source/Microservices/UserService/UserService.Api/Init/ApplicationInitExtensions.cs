using Common.Options;
using Common.Persistence;
using FluentValidation;
using UserService.Application.Commands;
using UserService.Application.Commands.Validators;
using UserService.Application.Interfaces;
using UserService.Application.Repository;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Persistence.Repositories;
using UserService.Infrastructure.Services;

namespace UserService.Api.Init;

public static class ApplicationInitExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IUnitOfWork, UnitOfWork<UserDbContext>>();

        services.AddScoped<RegisterUserCommandHandler>();
        services.AddScoped<LoginUserCommandHandler>();

        services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();

        return services;
    }
}
