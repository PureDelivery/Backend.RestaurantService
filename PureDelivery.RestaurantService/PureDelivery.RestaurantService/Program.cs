using PureDelivery.Common.Configuration.Extensions;
using PureDelivery.Common.Http.Extensions;
using PureDelivery.Infrastructure.Redis.Extensions;
using PureDelivery.RestaurantService.Helpers;
using Restaurant.Application.Builder;
using Restaurant.Application.Builder.impl;
using Restaurant.Application.Mappers;
using Restaurant.Application.Mappers.impl;
using Restaurant.Application.Pagination;
using Restaurant.Application.Pagination.impl;
using Restaurant.Application.Repositories;
using Restaurant.Application.Services;
using Restaurant.Application.Services.External;
using Restaurant.Application.Services.External.impl;
using Restaurant.Application.Services.impl;
using Restaurant.Application.Sorting;
using Restaurant.Application.Sorting.impl;
using Restaurant.Infrastructure.Repositories;
using Serilog;
using System.ComponentModel;
using EnumConverterImpl = Restaurant.Application.Mappers.impl.EnumConverter;
using RestaurantServiceImpl = Restaurant.Application.Services.impl.RestaurantService;

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

builder.Services.AddApiClient("HttpClient");

builder.Services.AddScoped<ILocationRequestMapper, LocationRequestMapper>();
builder.Services.AddScoped<ILocationResponseMapper, LocationResponseMapper>();
builder.Services.AddScoped<IDeliveryTimeCalculator, DeliveryTimeCalculator>();

// Request Builder
builder.Services.AddScoped<IHttpRequestBuilder, LocationServiceRequestBuilder>();

// Client
builder.Services.AddScoped<ILocationServiceClient, LocationServiceClient>();

// Register repositories
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();

// Register application services
builder.Services.AddScoped<IEnumConverter, EnumConverterImpl>();
builder.Services.AddScoped<IMenuMapper, MenuMapper>();
builder.Services.AddScoped<IRestaurantMapper, RestaurantMapper>();

builder.Services.AddScoped<IPaginationService, PaginationService>();
builder.Services.AddScoped<IRestaurantScoringService, RestaurantScoringService>();
builder.Services.AddScoped<IRestaurantSortingService, RestaurantSortingService>();

builder.Services.AddScoped<IRestaurantService, RestaurantServiceImpl>();
builder.Services.AddScoped<IMenuService, MenuService>();

await IoCHelper.ConfigureDatabaseAsync(builder);


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "PureDelivery Identity Service API",
        Version = "v1",
        Description = "API дл€ управлени€ клиентами и аутентификацией",
        Contact = new()
        {
            Name = "PureDelivery Team",
            Email = "support@puredelivery.com"
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Service API v1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
