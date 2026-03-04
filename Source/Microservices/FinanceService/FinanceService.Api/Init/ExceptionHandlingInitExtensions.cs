using FinanceService.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace FinanceService.Api.Init;

public static class ExceptionHandlingInitExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = feature?.Error;

                context.Response.ContentType = "application/json";

                switch (exception)
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

                await context.Response.WriteAsJsonAsync(new { error = exception?.Message });
            });
        });

        return app;
    }
}
