using Basket.Basket.Dtos;
using Basket.Data.Repository;
using FluentValidation;
using Shared.CQRS;

namespace Basket.Basket.Features.AddItemToBasket;

public record AddItemIntoBasketCommand(string UserName, ShoppingCartItemDto ShoppingCartItem)
    : ICommand<AddItemIntoBasketResult>;
public record AddItemIntoBasketResult(Guid Id);

public class AddItemIntoBasketCommandValidator : AbstractValidator<AddItemIntoBasketCommand>
{
    public AddItemIntoBasketCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(x => x.ShoppingCartItem.ProductId).NotEmpty().WithMessage("ProductId is required");
        RuleFor(x => x.ShoppingCartItem.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}

public class AddItemIntoBasketHandler
    : ICommandHandler<AddItemIntoBasketCommand, AddItemIntoBasketResult>
{
    private readonly IBasketRepository _basketRepository;
    public AddItemIntoBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }
    
    public async Task<AddItemIntoBasketResult> Handle(AddItemIntoBasketCommand command, CancellationToken cancellationToken)
    {
        // tackliyor yani
        var shoppingCart = await _basketRepository.GetBasket(command.UserName,false,cancellationToken);
        
        shoppingCart.AddItem(
            command.ShoppingCartItem.ProductId,
            command.ShoppingCartItem.Quantity,
            command.ShoppingCartItem.Color,
            command.ShoppingCartItem.Price,
            command.ShoppingCartItem.ProductName
            );
     
        // takip etme dedik get işleminde ondan oldu ??
        await _basketRepository.SaveChangesAsync(command.UserName,cancellationToken);

        return new AddItemIntoBasketResult(shoppingCart.Id);
    }
}