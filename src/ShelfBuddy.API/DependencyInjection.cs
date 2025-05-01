using MassTransit;
using ShelfBuddy.InventoryManagement.Application;
using ShelfBuddy.InventoryManagement.Infrastructure;

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
                cfg.ConfigureEndpoints(context);
            });
        });
    }
}