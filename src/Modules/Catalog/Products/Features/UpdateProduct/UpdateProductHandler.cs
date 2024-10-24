using Catalog.Data;
using Catalog.Products.Dtos;
using Catalog.Products.Exceptions;
using Catalog.Products.Models;
using Shared.CQRS;

namespace Catalog.Products.Features.UpdateProduct;

public record UpdateProductCommand(ProductDto Product)
    : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

public class UpdateProductHandler(CatalogDbContext dbContext)
    : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        // eger update icin vs. getiriyorsan findAsync çünkü keye göre getirir // diger turlu firstOrDefault olur
        var product = await dbContext.Products
            .FindAsync([command.Product.Id], cancellationToken: cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(command.Product.Id);
        }

        UpdateProductWithNewValues(product, command.Product);

        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
    
    private void UpdateProductWithNewValues(Product product, ProductDto productDto)
    {
        product.Update(
            productDto.Name,
            productDto.Category,
            productDto.Description,
            productDto.ImageFile,
            productDto.Price);
    }
}