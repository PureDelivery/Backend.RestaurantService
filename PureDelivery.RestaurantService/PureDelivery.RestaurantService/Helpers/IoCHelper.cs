using Microsoft.EntityFrameworkCore;
using PureDelivery.Common.Configuration.Services;
using RestaurantService.Infrastructure.Data;
using RestaurantService.Domain.Configuration;

namespace PureDelivery.RestaurantService.Helpers
{
    public static class IoCHelper
    {
        /// <summary>
        /// Конфигурация базы данных
        /// </summary>
        public static async Task ConfigureDatabaseAsync(WebApplicationBuilder builder)
        {
            var dbConfig = await LoadDatabaseConfigurationAsync(builder);

            builder.Services.AddDbContext<RestaurantDbContext>(options =>
            {
                ConfigureSqlServer(options, dbConfig);
                ConfigureLogging(options, dbConfig);
            });

            LogDatabaseConfiguration(dbConfig);
        }

        /// <summary>
        /// Загрузка конфигурации базы данных
        /// </summary>
        static async Task<RestaurantServiceConfig> LoadDatabaseConfigurationAsync(WebApplicationBuilder builder)
        {
            using var tempServiceProvider = builder.Services.BuildServiceProvider();

            var configProvider = tempServiceProvider.GetRequiredService<ICustomConfigurationProvider>();
            return await configProvider.GetConfigurationAsync<RestaurantServiceConfig>("RestaurantService");
        }

        /// <summary>
        /// Конфигурация SQL Server
        /// </summary>
        static void ConfigureSqlServer(DbContextOptionsBuilder options, RestaurantServiceConfig config)
        {
            options.UseSqlServer(config.ConnectionString, sqlOptions =>
            {
                if (config.Database.EnableRetryOnFailure)
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: config.Database.MaxRetryCount,
                        maxRetryDelay: TimeSpan.FromSeconds(config.Database.MaxRetryDelay),
                        errorNumbersToAdd: null);
                }

                sqlOptions.CommandTimeout(config.Database.CommandTimeout);
            });
        }

        /// <summary>
        /// Конфигурация логирования EF
        /// </summary>
        static void ConfigureLogging(DbContextOptionsBuilder options, RestaurantServiceConfig config)
        {
            if (config.Database.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        }

        /// <summary>
        /// Логирование конфигурации БД
        /// </summary>
        static void LogDatabaseConfiguration(RestaurantServiceConfig config)
        {
            Console.WriteLine($"   Restaurant Service Database Configuration:");
            Console.WriteLine($"   Source: {config.Database.EnableSensitiveDataLogging}");
            Console.WriteLine($"   Command Timeout: {config.Database.EnableSensitiveDataLogging}s");
            Console.WriteLine($"   Retry Enabled: {config.Database.EnableSensitiveDataLogging}");
            Console.WriteLine($"   Max Retry Count: {config.Database.EnableSensitiveDataLogging}");
            Console.WriteLine($"   Max Retry Delay: {config.Database.EnableSensitiveDataLogging}s");
            Console.WriteLine($"   Sensitive Logging: {config.Database.EnableSensitiveDataLogging}");
        }
    }
}
