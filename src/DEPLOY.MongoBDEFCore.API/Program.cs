using Asp.Versioning;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using DEPLOY.MongoBDEFCore.API.Configs;
using DEPLOY.MongoBDEFCore.API.Endpoints;
using DEPLOY.MongoBDEFCore.API.Infra.Database.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

const string serviceName = "canalDEPLOY.MongoBD.EFCore.API";

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

//Apenas para mostrar como seria o mapeamento utilizando o MongoDB
//SetupMap.ConfigureMaps();

builder.Services.AddEntityFrameworkMongoDB()
    .AddDbContext<MongoDBContext>(options =>
    {
        options.UseMongoDB(
            builder.Configuration.GetSection("Database:ConnectionString").Value!,
            builder.Configuration.GetSection("Database:DatabaseName").Value!);
        options.LogTo(Console.WriteLine);
        options.EnableSensitiveDataLogging();
    });

builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource =>
        {
            resource.AddService(serviceName);
            resource.AddAttributes(new Dictionary<string, object>
            {
                { "service.name", serviceName },
                { "service.version", "1.0.0" },
                { "service.instance.id", Environment.MachineName }
            });
        })
         // .UseAzureMonitor(configureAzureMonitor =>
         // {
         //     var connectionString = builder.Configuration.GetSection("ApplicationInsights:ConnectionString").Value;
         //     if (!string.IsNullOrEmpty(connectionString))
         //     {
         //         configureAzureMonitor.ConnectionString = connectionString;
         //         configureAzureMonitor.EnableLiveMetrics = true;
         //     }
         // })
         .WithTracing(tracing => tracing
             .AddAspNetCoreInstrumentation()
             .AddHttpClientInstrumentation()
             .AddConsoleExporter()
             .AddOtlpExporter(options =>
             {
                 options.Endpoint = new Uri("http://localhost:4318/v1/traces");
                 options.Protocol = OtlpExportProtocol.HttpProtobuf;
                 options.ExportProcessorType = ExportProcessorType.Batch;
             }))
         .WithMetrics(metrics => metrics
             .AddAspNetCoreInstrumentation()
             .AddOtlpExporter(options =>
             {
                 options.Endpoint = new Uri("http://localhost:4318/v1/metrics");
                 options.Protocol = OtlpExportProtocol.HttpProtobuf;
                 options.ExportProcessorType = ExportProcessorType.Batch;
             })
             .AddHttpClientInstrumentation()
             .AddConsoleExporter());

builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName))
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://localhost:4318/v1/logs");
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
            options.ExportProcessorType = ExportProcessorType.Simple;
        });
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
