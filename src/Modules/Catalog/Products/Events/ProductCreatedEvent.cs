using Catalog.Products.Models;
using Shared.DDD;

namespace Catalog.Products.Events;

// record olmazsa taniyamiyor class olmicak yani
public record ProductCreatedEvent(Product Product)
    : IDomainEvent;