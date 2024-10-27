using System.Reflection;
using Catalog.Data;
using Catalog.Seed;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Behaviors;
using Shared.Data;
using Shared.Data.Interceptors;
using Shared.Data.Seed;

namespace Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        // add services to collection

        var connectionString = configuration.GetConnectionString("Database");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();
        
        // sp => ServiceProvider
        services.AddDbContext<CatalogDbContext>((sp,options) =>
        {
            // mediatr eklenmezse bu calismazdi 36
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IDataSeeder, CatalogDataSeeder>();
        
        return services;
    }


    // buraya ulasmak icin fluentValidation.AspnetCore eklendi -- bu sekilde tanındı IApplicationBuilder
    public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
    {
        // oto db ayakta ise migration alcak artık dotnet ef update db gerek yok
        app.UseMigration<CatalogDbContext>();

        return app;
    }
}