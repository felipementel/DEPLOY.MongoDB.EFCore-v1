using Asp.Versioning;
using DEPLOY.MongoBDEFCore.API.Domain;
using DEPLOY.MongoBDEFCore.API.Infra.Database.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using System.Runtime.CompilerServices;

namespace DEPLOY.MongoBDEFCore.API.Endpoints
{
    public static class MarinasEndpoints
    {
        public static void MapMarinasEndpoints(this IEndpointRouteBuilder app)
        {
            var apiVersionSetAdocoes = app
                .NewApiVersionSet("marinas")
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            var adocoes = app
                .MapGroup("/api/v{apiVersion:apiVersion}/marinas")
                .RequireAuthorization()
                .WithApiVersionSet(apiVersionSetAdocoes);



            app.MapPost("/marina", async (MongoDBContext context, Marina marina) =>
            {
                context.Marinas.Add(new Marina { Name = marina.Name });
                await context.SaveChangesAsync();

                return TypedResults.Created($"/marina/id/{marina.Id}", marina);
            })
            .Produces(201)
            .Produces(401)
            .Produces(422)
            .Produces(500)
            .WithOpenApi(operation => new(operation)
            {
                OperationId = "marina-post",
                Summary = "create a marina",
                Description = "process do register a new marina",
                Tags = new List<OpenApiTag> { new() { Name = "Marina" } }
            });

            app.MapGet("/marina", GetMarina)
            .Produces(201)
            .Produces(401)
            .Produces(422)
            .Produces(500)
            .WithOpenApi(operation => new(operation)
            {
                OperationId = "marina-get",
                Summary = "get a marina",
                Description = "process do get a marina",
                Tags = new List<OpenApiTag> { new() { Name = "marina" } }
            });

            app.MapGet("/marina/all", async (MongoDBContext context) =>
            {
                var items = await context.Marinas.ToListAsync();

                return TypedResults.Ok(items);
            });

            app.MapDelete("/marina/{marinaName}", async
                (MongoDBContext context,
                string marinaName,
                CancellationToken cancellationToken = default) =>
            {
                var marina = await context.Marinas.FirstOrDefaultAsync(x => x.Name == marinaName, cancellationToken);

                if (marina == null)
                {
                    return Results.NotFound();
                }

                context.Marinas.Remove(marina);
                await context.SaveChangesAsync();

                return TypedResults.Ok();
            });

            app.MapDelete("/marina/id/{id}", async
                (MongoDBContext context,
                string id,
                CancellationToken cancellationToken = default) =>
            {
                var marina = await context.Marinas.FirstOrDefaultAsync(x => x.Id == ObjectId.Parse(id), cancellationToken);

                if (marina == null)
                {
                    return Results.NotFound();
                }

                context.Marinas.Remove(marina);
                await context.SaveChangesAsync();

                return TypedResults.Ok();
            });

            app.MapPut("/marina/{marinaName}", async (
                MongoDBContext context,
                string marinaName,
                CancellationToken cancellationToken = default) =>
            {
                var marina = await context.Marinas.FirstOrDefaultAsync(x => x.Name == marinaName, cancellationToken);

                if (marina == null)
                {
                    return Results.NotFound();
                }

                await context.SaveChangesAsync();
                return TypedResults.Ok();
            });

            async Task<IResult> GetMarina(MongoDBContext context, string marinaName)
            {
                var marina = await context.Marinas.FirstOrDefaultAsync(x => x.Name == marinaName);

                if (marina == null)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.Ok();
            }
        }
    }
}
