using MassTransit;
using Microsoft.Extensions.Hosting;

namespace ShelfBuddy.InventoryManagement.Application;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInventoryManagementApplication(this IHostApplicationBuilder builder)
    {
        return builder;
    }

    public static IBusRegistrationConfigurator AddInventoryManagementConsumers(this IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumersFromNamespaceContaining(typeof(DependencyInjection));
        return configurator;
    }
}