using DEPLOY.MongoBDEFCore.API.Configs;
using DEPLOY.MongoBDEFCore.API.Endpoints;
using DEPLOY.MongoBDEFCore.API.Infra.Database.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.IncludeFields = true;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

var itemConfig = builder.Services
    .AddOptions<BoatDatabaseSettings>()
    .BindConfiguration("BoatDatabase")
    .ValidateDataAnnotations()
    .ValidateOnStart()
    .Validate(config =>
    {
        if (config is null || config.ConnectionString is null || config.DatabaseName is null)
        {
            throw new Exception("Database is not configured");
        }
        return true;
    });

builder.Services.AddOpenApi();

SetupMap.ConfigureMaps();

builder.Services.AddEntityFrameworkMongoDB()
    .AddDbContext<MongoDBContext>(options =>
    {
        options.UseMongoDB(builder.Configuration.GetSection("BoatDatabase:ConnectionString").Value!, builder.Configuration.GetSection("BoatDatabase:DatabaseName").Value!);
        options.LogTo(Console.WriteLine);
        options.EnableSensitiveDataLogging();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    app.UseHttpsRedirection();
}

//Endpoint
app.MapBoatEndpoints();

await app.RunAsync();
