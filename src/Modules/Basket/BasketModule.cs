using Basket.Data;
using Basket.Data.Processors;
using Basket.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Data.Interceptors;

namespace Basket;

public static class BasketModule
{
    public static IServiceCollection AddBasketModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        // add services to collection
        // bir interface'i ikisi içinde kullanmak doğru değildir ondan ... Decorated class =>> cachebasket => Basketreposunun modifiyeli hali
        services.AddScoped<IBasketRepository, BasketRepository>();
        
        // bu manuel decoration'dan daha iyi < aynı mantık ama Scrutor,,, DIAbstraction 8.0.1 >= ister
        services.Decorate<IBasketRepository, CachedBasketRepository>();
        
        var connectionString = configuration.GetConnectionString("Database");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();
        
        // sp => ServiceProvider
        services.AddDbContext<BasketDbContext>((sp,options) =>
        {
            // mediatr eklenmezse bu calismazdi 36
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddHostedService<OutboxProcessor>();
        
        return services;
    }
    
    public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
    {
        app.UseMigration<BasketDbContext>();

        return app;
    }
}