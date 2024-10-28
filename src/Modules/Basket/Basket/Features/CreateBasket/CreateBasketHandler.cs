using Basket.Basket.Dtos;
using Basket.Basket.Models;
using Basket.Data.Repository;
using FluentValidation;
using Shared.Contracts.CQRS;

namespace Basket.Basket.Features.CreateBasket;

public record CreateBasketCommand(ShoppingCartDto ShoppingCart)
    : ICommand<CreateBasketResult>;
public record CreateBasketResult(Guid Id);

public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
{
    public CreateBasketCommandValidator()
    {
        RuleFor(x => x.ShoppingCart.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class CreateBasketHandler
    : ICommandHandler<CreateBasketCommand, CreateBasketResult>
{
    private readonly IBasketRepository _basketRepository;
    public CreateBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }
    
    public async Task<CreateBasketResult> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
    {
        var shoppingCart = CreateNewBasket(command.ShoppingCart);        

        await _basketRepository.CreateBasket(shoppingCart,cancellationToken);
        
        return new CreateBasketResult(shoppingCart.Id);
    }
    
    private ShoppingCart CreateNewBasket(ShoppingCartDto shoppingCartDto)
    {
        // create new basket
        var newBasket = ShoppingCart.Create(
            Guid.NewGuid(),
            shoppingCartDto.UserName);

        shoppingCartDto.Items.ForEach(item =>
        {
            newBasket.AddItem(
                item.ProductId,
                item.Quantity,
                item.Color,
                item.Price,
                item.ProductName);
        });

        return newBasket;
    }
}