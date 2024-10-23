using Catalog.Products.Models;
using Shared.DDD;

namespace Catalog.Products.Events;

public class ProductPriceChangedEvent(Product product)
    : IDomainEvent;