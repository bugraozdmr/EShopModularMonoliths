using Carter;
using Serilog;
using Shared.Exceptions.Handler;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

//common services: carter, mediatr, fluentvalidation, masstransit

var catalogAssembly = typeof(CatalogModule).Assembly;
var basketAssembly = typeof(BasketModule).Assembly; 
//var orderingAssembly = typeof(OrderingModule).Assembly;

// yeni moduller gelince daha kullanisli oldugu anlasilir
builder.Services.AddCarterWithAssemblies(
    catalogAssembly,basketAssembly);

// fluent valation'da icerde
builder.Services.AddMediatRWithAssemblies(
    catalogAssembly, basketAssembly);



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
//app.UseAuthentication();
//app.UseAuthorization();

app
    .UseCatalogModule()
    .UseBasketModule()
    .UseOrderingModule();

app.Run();