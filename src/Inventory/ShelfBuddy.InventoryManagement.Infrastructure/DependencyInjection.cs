using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShelfBuddy.InventoryManagement.Application;
using ShelfBuddy.InventoryManagement.Infrastructure.Persistence;

namespace ShelfBuddy.InventoryManagement.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInventoryManagementInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Services.AddInventoryManagementInfrastructureServices();
        builder.AddPersistence();
        return builder;
    }

    private static void AddInventoryManagementInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IInventoryRepository, InventoryRepository>();
    }

    private static IHostApplicationBuilder AddPersistence(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<InventoryDbContext>("ShelfBuddyDb");
        return builder;
    }
}