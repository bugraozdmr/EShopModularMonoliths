using System.Text.Json;
using Basket.Basket.Dtos;
using Basket.Basket.Exceptions;
using Basket.Basket.Models;
using Basket.Data;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.CQRS;
using Shared.Messaging.Events;

namespace Basket.Basket.Features.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckout)
    : ICommand<CheckoutBasketResult>;
public record CheckoutBasketResult(bool IsSuccess);
public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.BasketCheckout).NotNull().WithMessage("BasketCheckoutDto can't be null");
        RuleFor(x => x.BasketCheckout.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class CheckoutBasketHandler
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    // INJECTION WE ARE VENOM // we need to work on that :D
    private readonly BasketDbContext _dbContext;
    public CheckoutBasketHandler(BasketDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get existing basket with total price
            var basket = await _dbContext.ShoppingCarts
                .Include(x => x.Items)
                .SingleOrDefaultAsync(x => x.UserName == command.BasketCheckout.UserName, cancellationToken);

            if (basket == null)
            {
                throw new BasketNotFoundException(command.BasketCheckout.UserName);
            }

            // Set total price on basket checkout event message
            var eventMessage = command.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>();
            eventMessage.TotalPrice = basket.TotalPrice;
            
            // Write a message to the outbox
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = typeof(BasketCheckoutIntegrationEvent).AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(eventMessage),
                OccuredOn = DateTime.UtcNow
            };
            
            _dbContext.OutboxMessages.Add(outboxMessage);

            // Delete the basket
            _dbContext.ShoppingCarts.Remove(basket);

            await _dbContext.SaveChangesAsync(cancellationToken);
            
            // 2 kere yormuyor islemler tamamlaninca buraya gelir commit ediyor
            await transaction.CommitAsync(cancellationToken);

            return new CheckoutBasketResult(true);
        }
        catch
        {
            // hatada rollback ediyor
            await transaction.RollbackAsync(cancellationToken);
            return new CheckoutBasketResult(false);
        }
        
        
        ///////////////////// CHECKOUT BASKET WITHOUT OUTBOX
        //var basket =
        //    await repository.GetBasket(command.BasketCheckout.UserName, true, cancellationToken);

        //var eventMessage = command.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>();
        //eventMessage.TotalPrice = basket.TotalPrice;

        // birinde rabbitmq diğerinde repository(postgreSql) implement edilmiş dual write biri patlarsa diperide patlayabilir sıkıntı çıkar
        //await bus.Publish(eventMessage, cancellationToken);

        //await repository.DeleteBasket(command.BasketCheckout.UserName, cancellationToken);

        //return new CheckoutBasketResult(true);
        ///////////////////// CHECKOUT BASKET WITHOUT OUTBOX
    }
}