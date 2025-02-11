using DEPLOY.MongoBDEFCore.API;
using DEPLOY.MongoBDEFCore.API.Configs;
using DEPLOY.MongoBDEFCore.API.Domain;
using DEPLOY.MongoBDEFCore.API.Endpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.IncludeFields = true;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    // Adicione outras opções de configuração conforme necessário
});

//var item = builder.Services.Configure<BoatDatabaseSettings>(
//    builder.Configuration.GetSection("BoatDatabase"));

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
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

//Endpoint
app.MapBoatEndpoints();

await app.RunAsync();


//
public static class SetupMap
{
    public static void ConfigureMaps()
    {
        BoatEntityMap.Configure();
        MarinaEntityMap.Configure();
        // outras classes
    }
}

public static class BoatEntityMap
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Boat>(map =>
        {
            map.AutoMap();

            //map.SetIgnoreExtraElements(true);

            map
            .MapIdProperty(i => i.Id)
            .SetElementName("_id")
            .SetIdGenerator(StringObjectIdGenerator.Instance);

            map.
            MapMember(m => m.Name)
            .SetElementName("name");

            map.
            MapMember(m => m.Size)
            .SetElementName("size");

            map.
            MapMember(m => m.License)
            .SetElementName("license");
        });
    }
}

public static class MarinaEntityMap
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Marina>(map =>
        {
            map.AutoMap();

            map
            .MapIdProperty(i => i.Id)
            .SetElementName("_id")
            .SetIdGenerator(StringObjectIdGenerator.Instance);

            map.
            MapMember(m => m.Name)
            .SetElementName("name");
        });
    }
}