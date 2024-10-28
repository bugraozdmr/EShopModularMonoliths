using Catalog.Data;
using Catalog.Products.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.CQRS;

namespace Catalog.Products.Features.GetProductById;

// public record GetProductByIdQuery(Guid Id)
//     : IQuery<GetProductByIdResult>;
// public record GetProductByIdResult(ProductDto Product);

// sadece expose olan metodlar contractlara taşındı diğerleri zaten modüller arası etkileşim gerektirmediği için kaldı

// contract related işlemler için contracts'a referans verildi
public class GetProductByIdHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(query.Id);
        }

        //mapping product entity to productdto
        var productDto = product.Adapt<ProductDto>();

        return new GetProductByIdResult(productDto);
    }
}