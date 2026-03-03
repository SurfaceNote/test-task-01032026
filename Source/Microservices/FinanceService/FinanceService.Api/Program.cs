using System.Text;
using Common.Options;
using Common.Persistence;
using FinanceService.Application.Commands;
using FinanceService.Application.Commands.Validators;
using FinanceService.Application.Exceptions;
using FinanceService.Application.Queries;
using FinanceService.Application.Repositories;
using FinanceService.Infrastructure.Persistence;
using FinanceService.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("UserDb");

builder.Services.AddDbContext<FinanceDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwt = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,

            ValidateAudience = true,
            ValidAudience = jwt.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IFavoriteCurrencyRepository, FavoriteCurrencyRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork<FinanceDbContext>>();

builder.Services.AddScoped<GetAllCurrenciesQueryHandler>();
builder.Services.AddScoped<AddFavoriteCurrencyCommandHandler>();
builder.Services.AddScoped<GetUserFavoriteCurrenciesQueryHandler>();
builder.Services.AddScoped<RemoveFavoriteCurrencyCommandHandler>();

builder.Services.AddValidatorsFromAssemblyContaining<AddFavoriteCurrencyCommandValidator>();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        context.Response.ContentType = "application/json";

        switch (ex)
        {
            case ValidationException validationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Ошибка валидации",
                    errors = validationException.Errors.Select(error => new
                    {
                        field = error.PropertyName,
                        message = error.ErrorMessage
                    })
                });
                return;
            
            case CurrencyNotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                break;
            
            case FavoriteCurrencyAlreadyExistsException:
            case FavoriteCurrencyDoesntExistsException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        await context.Response.WriteAsJsonAsync(new { error = ex?.Message });
    });
});

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();