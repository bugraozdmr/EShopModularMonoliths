using Basket.Basket.Dtos;
using Basket.Data.Repository;
using Catalog.Contracts.Products.Features.GetProductById;
using FluentValidation;
using MediatR;
using Shared.Contracts.CQRS;

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
    private readonly ISender _sender;
    public AddItemIntoBasketHandler(IBasketRepository basketRepository, ISender sender)
    {
        _basketRepository = basketRepository;
        _sender = sender;
    }
    
    public async Task<AddItemIntoBasketResult> Handle(AddItemIntoBasketCommand command, CancellationToken cancellationToken)
    {
        // tackliyor yani
        var shoppingCart = await _basketRepository.GetBasket(command.UserName,false,cancellationToken);
        
        
        var result = await _sender.Send(
            new GetProductByIdQuery(command.ShoppingCartItem.ProductId));
        
        // istek atip ordan fiyati ve urunu getirme
        shoppingCart.AddItem(
            command.ShoppingCartItem.ProductId,
            command.ShoppingCartItem.Quantity,
            command.ShoppingCartItem.Color,
            result.Product.Price,
            result.Product.Name);
            // command.ShoppingCartItem.Price,
            // command.ShoppingCartItem.ProductName
            // );
     
        // takip etme dedik get işleminde ondan oldu ??
        await _basketRepository.SaveChangesAsync(command.UserName,cancellationToken);

        return new AddItemIntoBasketResult(shoppingCart.Id);
    }
}