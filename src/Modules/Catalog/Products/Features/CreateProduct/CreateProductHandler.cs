using Catalog.Data;
using Catalog.Products.Dtos;
using Catalog.Products.Models;
using MediatR;
using Shared.CQRS;

namespace Catalog.Products.Features.CreateProduct;

public record CreateProductCommand(ProductDto Product) 
     : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductHandler(CatalogDbContext dbContext) 
     : IRequestHandler<CreateProductCommand, CreateProductResult>
{
     public async Task<CreateProductResult> Handle(CreateProductCommand command,
          CancellationToken cancellationToken)
     {
          // Maplemek gibi dusun ama rich domain oldugu icin gerekli
          var product = CreateNewProduct(command.Product);

          dbContext.Products.Add(product);
          await dbContext.SaveChangesAsync(cancellationToken);

          return new CreateProductResult(product.Id);
     }
     
     private Product CreateNewProduct(ProductDto productDto)
     {
          var product = Product.Create(
               Guid.NewGuid(),
               productDto.Name,
               productDto.Category,
               productDto.Description,
               productDto.ImageFile,
               productDto.Price);

          return product;
     }
}