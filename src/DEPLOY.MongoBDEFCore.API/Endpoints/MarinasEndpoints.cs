using Asp.Versioning;
using DEPLOY.MongoBDEFCore.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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


            app.MapPost("/marinas", async (MongoDBContext context, Marina marina) =>
            {
                context.Marinas.Add(new Marina { Name = marina.Name });
                await context.SaveChangesAsync();

                return TypedResults.Created();
            })
            .Produces(201)
            .Produces(401)
            .Produces(422)
            .Produces(500)
            .WithOpenApi(operation => new(operation)
            {
                OperationId = "Marina-post",
                Summary = "create a Marina",
                Description = "process do register a new Marina",
                Tags = new List<OpenApiTag> { new() { Name = "Marina" } }
            });

            app.MapGet("/Marina", GetMarina)
            .Produces(201)
            .Produces(401)
            .Produces(422)
            .Produces(500)
            .WithOpenApi(operation => new(operation)
            {
                OperationId = "Marina-get",
                Summary = "get a Marina",
                Description = "process do get a Marina",
                Tags = new List<OpenApiTag> { new() { Name = "Marina" } }
            });

            app.MapGet("/Marina/all", async (MongoDBContext context) =>
            {
                var items = await context.Marinas.ToListAsync();

                return TypedResults.Ok(items);
            });

            app.MapDelete("/Marina/{MarinaName}", async (MongoDBContext context, string MarinaName) =>
            {
                var Marina = await context.Marinas.FirstOrDefaultAsync(x => x.Name == MarinaName);
                context.Marinas.Remove(Marina);
                await context.SaveChangesAsync();

                return TypedResults.Ok();
            });

            app.MapPut("/Marina/{MarinaName}", async (MongoDBContext context, string MarinaName) =>
            {
                var Marina = await context.Marinas.FirstOrDefaultAsync(x => x.Name == MarinaName);

                if (Marina == null)
                {
                    return Results.NotFound();
                }

                Marina.Name = "New Name";
                await context.SaveChangesAsync();

                return TypedResults.Ok();
            });

            async Task<IResult> GetMarina (MongoDBContext context, string boarName)
            {
                var Marina = await context.Marinas.FirstOrDefaultAsync(x => x.Name == boarName);

                if (Marina == null)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.Ok();
            }
        }
    }
}
