using MassTransit;
using ShelfBuddy.InventoryManagement.Application;
using ShelfBuddy.InventoryManagement.Infrastructure;
using ShelfBuddy.SharedKernel.Json;

namespace ShelfBuddy.API;

public static class DependencyInjection
{
    public static void AddShelfBuddyServices(this IHostApplicationBuilder builder)
    {
        builder
            .AddInventoryManagementApplication()
            .AddInventoryManagementInfrastructure();

        builder.Services.AddMessaging();
    }

    private static void AddMessaging(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddInventoryManagementConsumers();
            
            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureJsonSerializerOptions(options =>
                {
                    options.Converters.Add(new ErrorOrConverterFactory());
                    return options;
                });
                cfg.ConfigureEndpoints(context);
            });
        });
    }
}