using PureDelivery.Common.Configuration.Extensions;
using PureDelivery.Infrastructure.Redis.Extensions;
using PureDelivery.RestaurantService.Helpers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.WithProperty("Service", "RestaurantService");
});

// Add services to the container.
builder.Services.AddRedisServices("Redis");
builder.Services.AddConfigurationProvider(builder.Configuration);


await IoCHelper.ConfigureDatabaseAsync(builder);


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
