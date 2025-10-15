using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShelfBuddy.ClientInterface.LocalDb;

var builder = new HostApplicationBuilder();
builder.Services.AddSqlite<LocalDbContext>(
    $"Data Source={Path.Combine(AppContext.BaseDirectory, @"shelfbuddy.db")}");

builder.Build().Run();