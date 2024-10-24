using Catalog.Products.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Products.EventHandlers;

public class ProductPriceChangedEventHandler
    (ILogger<ProductPriceChangedEventHandler> logger)
    : INotificationHandler<ProductPriceChangedEvent>
{
    public Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);        

        return Task.CompletedTask;
        /*
        // Publish product price changed integration event for update basket prices
        var integrationEvent = new ProductPriceChangedIntegrationEvent
        {
            ProductId = notification.Product.Id,
            Name = notification.Product.Name,
            Category = notification.Product.Category,
            Description = notification.Product.Description,
            ImageFile = notification.Product.ImageFile,
            Price = notification.Product.Price //set updated product price
        };

        await bus.Publish(integrationEvent, cancellationToken);*/
    }
}