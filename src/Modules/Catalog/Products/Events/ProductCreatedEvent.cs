using Catalog.Products.Models;
using Shared.DDD;

namespace Catalog.Products.Events;

public class ProductCreatedEvent(Product product)
    : IDomainEvent;