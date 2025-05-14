using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var dbServer = builder.AddAzureSqlServer("ShelfBuddy-SQL");

if (builder.Environment.IsDevelopment())
{
    dbServer = dbServer.RunAsContainer(sqlContainer =>
    {
        sqlContainer
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent);
    });
}

var db = dbServer.AddDatabase("ShelfBuddyDb", "ShelfBuddy");

builder
    .AddProject<Projects.ShelfBuddy_API>("ShelfBuddy-API")
    .WaitFor(dbServer)
    .WithReference(db)
    .WithExternalHttpEndpoints();

builder.Build().Run();
