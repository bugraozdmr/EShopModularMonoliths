using Catalog.Contracts.Products.Dtos;
using Shared.Contracts.CQRS;

namespace Catalog.Contracts.Products.Features.GetProductById;

// TODO moduller arasi iletisim icin contractlar kullaniliyor

public record GetProductByIdQuery(Guid Id)
    : IQuery<GetProductByIdResult>;
public record GetProductByIdResult(ProductDto Product);