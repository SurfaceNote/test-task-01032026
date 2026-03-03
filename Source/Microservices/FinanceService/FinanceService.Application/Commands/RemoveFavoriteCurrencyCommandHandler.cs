using Common.Persistence;
using FinanceService.Application.Exceptions;
using FinanceService.Application.Repositories;
using FluentValidation;

namespace FinanceService.Application.Commands;

public class RemoveFavoriteCurrencyCommandHandler(
    IFavoriteCurrencyRepository favoriteCurrencyRepository, 
    IValidator<RemoveFavoriteCurrencyCommand> commandValidator, 
    IUnitOfWork unitOfWork)
{
    public async Task Handle(RemoveFavoriteCurrencyCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await commandValidator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        // Пробуем удалить запись из избранного
        var deleted = await favoriteCurrencyRepository.DeleteAsync(command.UserId, command.CurrencyId, cancellationToken);
        if (deleted)
        {
            // если получилось, сохраняем удаление
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            // бросаем ошибку, т.к. пытались удалить валюту, которая и не была в избранном
            throw new FavoriteCurrencyDoesntExistsException(command.UserId, command.CurrencyId);
        }
    }
}