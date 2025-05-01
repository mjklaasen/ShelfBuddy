using ShelfBuddy.API;
using ShelfBuddy.InventoryManagement.Application;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddShelfBuddyServices();

builder.Services.AddOpenApi();

var app = builder.Build();

app
    .MapDefaultEndpoints()
    .MapInventoryEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API");
    });
}

app.UseHttpsRedirection();

app.Run();
