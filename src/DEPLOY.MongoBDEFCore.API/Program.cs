using Asp.Versioning;
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

builder.Services.AddRouting(opt =>
{
    opt.LowercaseUrls = true;
    opt.LowercaseQueryStrings = true;
});

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";

        options.SubstituteApiVersionInUrl = true;
    })
.EnableApiVersionBinding();

builder.Services.AddEndpointsApiExplorer();

var itemConfig = builder.Services
    .AddOptions<DatabaseSettings>()
    .BindConfiguration("Database")
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
        options.UseMongoDB(
            builder.Configuration.GetSection("Database:ConnectionString").Value!,
            builder.Configuration.GetSection("Database:DatabaseName").Value!);
        options.LogTo(Console.WriteLine);
        options.EnableSensitiveDataLogging();
    });

var app = builder.Build();

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
app.MapMarinasEndpoints();

await app.RunAsync();