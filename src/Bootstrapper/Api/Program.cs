using Carter;
using Keycloak.AuthServices.Authentication;
using Serilog;
using Shared.Exceptions.Handler;
using Shared.Extensions;
using Shared.Messaging.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

//common services: carter, mediatr, fluentvalidation, masstransit

var catalogAssembly = typeof(CatalogModule).Assembly;
var basketAssembly = typeof(BasketModule).Assembly; 
var orderingAssembly = typeof(OrderingModule).Assembly;

// yeni moduller gelince daha kullanisli oldugu anlasilir
builder.Services.AddCarterWithAssemblies(
    catalogAssembly,basketAssembly,orderingAssembly);

// fluent valation'da icerde
builder.Services.AddMediatRWithAssemblies(
    catalogAssembly, basketAssembly,orderingAssembly);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services
    .AddMassTransitWithAssemblies(builder.Configuration, catalogAssembly, basketAssembly);

// KeyCloak
builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// module services
// Add Services to the container
builder.Services
    .AddCatalogModule(builder.Configuration)
    .AddBasketModule(builder.Configuration)
    .AddOrderingModule(builder.Configuration);


// bu handler diğerinden daha guzel daha detaylı bunu kullan
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();


// Configure the HTTP request pipeline
app.MapCarter();
app.UseSerilogRequestLogging();
app.UseExceptionHandler(options => { });
app.UseAuthentication();
app.UseAuthorization();

app
    .UseCatalogModule()
    .UseBasketModule()
    .UseOrderingModule();

app.Run();