using Basket.Data.Repository;
using FluentValidation;
using Shared.Contracts.CQRS;

namespace Basket.Basket.Features.RemoveItemFromBasket;

public record RemoveItemFromBasketCommand(string UserName, Guid ProductId)
    : ICommand<RemoveItemFromBasketResult>;
public record RemoveItemFromBasketResult(Guid Id);
public class RemoveItemFromBasketCommandValidator : AbstractValidator<RemoveItemFromBasketCommand>
{
    public RemoveItemFromBasketCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required");
    }
}

public class RemoveItemFromBasketHandler
    : ICommandHandler<RemoveItemFromBasketCommand, RemoveItemFromBasketResult>

{
    private readonly IBasketRepository _basketRepository;
    public RemoveItemFromBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }
    
    public async Task<RemoveItemFromBasketResult> Handle(RemoveItemFromBasketCommand command, CancellationToken cancellationToken)
    {
        // takip etme dedik
        var shoppingCart = await _basketRepository.GetBasket(command.UserName,false,cancellationToken);
        
        // sallamaz var mı yok diye olması gereken
        shoppingCart.RemoveItem(command.ProductId);
        
        await _basketRepository.SaveChangesAsync(command.UserName,cancellationToken);

        return new RemoveItemFromBasketResult(shoppingCart.Id);
    }
}


// When tracking is set to false, Entity Framework (EF) won’t monitor the retrieved entities for changes as it usually does with tracking. However, in your case, you’re still able to make changes to the shoppingCart entity and save them because of the way RemoveItem and SaveChangesAsync are functioning together.