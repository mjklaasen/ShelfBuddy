using System.Text.Json.Serialization;
using ShelfBuddy.API;
using ShelfBuddy.InventoryManagement.Application;
using ShelfBuddy.SharedKernel.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddShelfBuddyServices();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new ErrorOrConverterFactory());
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddOpenApi(options => options.ShouldInclude += description => !string.IsNullOrEmpty(description.GroupName));

var app = builder.Build();

app
    .MapDefaultEndpoints()
    .MapInventoryManagementEndpoints();

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
