using Catalog.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Seed;

namespace Catalog.Seed;

public class CatalogDataSeeder(CatalogDbContext dbContext)
    : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        if (!await dbContext.Products.AnyAsync())
        {
            await dbContext.Products.AddRangeAsync(InitialData.Products);
            
            // Bu oto domaine ne zaman üretildi vs. bilgilerini gömüypr initial datada vs. hepsinde
            await dbContext.SaveChangesAsync();
        }
    }
}