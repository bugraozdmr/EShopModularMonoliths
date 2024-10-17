using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        // add services to collection
        
        return services;
    }


    // buraya ulasmak icin fluentValidation.AspnetCore eklendi -- bu sekilde tanındı IApplicationBuilder
    public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
    {


        return app;
    }
}