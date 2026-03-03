using Common.Persistence;
using FinanceService.Application.Exceptions;
using FinanceService.Application.Repositories;
using FinanceService.Domain;
using FluentValidation;

namespace FinanceService.Application.Commands;

public class AddFavoriteCurrencyCommandHandler(
    IFavoriteCurrencyRepository favoriteCurrencyRepository, 
    ICurrencyRepository currencyRepository,
    IValidator<AddFavoriteCurrencyCommand> commandValidator,
    IUnitOfWork unitOfWork)
{
    public async Task Handle(AddFavoriteCurrencyCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await commandValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        // Проверяем, что валюта существует
        var currencyExists = await currencyRepository.ExistsByIdAsync(command.CurrencyId, cancellationToken);
        if (!currencyExists)
        {
            throw new CurrencyNotFoundException(command.CurrencyId);
        }
        
        // Проверяем, что эта валюта уже не была добавлена в избранное раньше
        var alreadyExists = await favoriteCurrencyRepository.ExistsAsync(command.UserId, command.CurrencyId, cancellationToken);
        if (alreadyExists)
        {
            throw new FavoriteCurrencyAlreadyExistsException(command.UserId, command.CurrencyId);
        }
        
        // Добавляем валюту в избранное для пользователя
        var favoriteCurrency = new UserFavoriteCurrency(command.UserId, command.CurrencyId);
        await favoriteCurrencyRepository.AddAsync(favoriteCurrency, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}