using Asp.Versioning;
using DEPLOY.MongoBDEFCore.API.Domain;
using DEPLOY.MongoBDEFCore.API.Infra.Database.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace DEPLOY.MongoBDEFCore.API.Endpoints
{
    public static class BoatsEndpoints
    {
        public static void MapBoatEndpoints(this IEndpointRouteBuilder app)
        {
            var apiVersionSetAdocoes = app
                .NewApiVersionSet("boats")
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            var adocoes = app
                .MapGroup("/api/v{apiVersion:apiVersion}/boats")
                .RequireAuthorization()
                .WithApiVersionSet(apiVersionSetAdocoes);


            app.MapPost("/boat", async (MongoDBContext context, 
                [FromBody] Boat boat) =>
            {
                context.Boats.Add(new Boat { Name = boat.Name, Size = boat.Size, License = true });
                await context.SaveChangesAsync();

                return TypedResults.Created($"/boat/id/{boat.Id}", boat);
            })
            .Produces(201)
            .Produces(401)
            .Produces(422)
            .Produces(500)
            .WithOpenApi(operation => new(operation)
            {
                OperationId = "boat-post",
                Summary = "create a boat",
                Description = "process do register a new boat",
                Tags = new List<OpenApiTag> { new() { Name = "Boat" } }
            });

            app.MapGet("/boat", GetBoat)
            .Produces(201)
            .Produces(401)
            .Produces(422)
            .Produces(500)
            .WithOpenApi(operation => new(operation)
            {
                OperationId = "boat-get",
                Summary = "get a boat",
                Description = "process do get a boat",
                Tags = new List<OpenApiTag> { new() { Name = "Boat" } }
            });

            app.MapGet("/boat/all", async (MongoDBContext context) =>
            {
                var items = await context.Boats.ToListAsync();

                return TypedResults.Ok(items);
            });

            app.MapGet("/boat/{boatName}", async (MongoDBContext context,
                [FromRoute] string boatName,
                CancellationToken cancellationToken = default) =>
            {
                var boat = await context.Boats.FirstOrDefaultAsync(x => x.Name == boatName, cancellationToken);

                if (boat == null)
                {
                    return Results.NotFound();
                }

                return TypedResults.Ok(boat);
            });

            app.MapGet("/boatbyid/{id}", async
                (MongoDBContext context,
                [FromRoute] string id,
                CancellationToken cancellationToken = default) =>
            {
                var boat = await context.Boats.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (boat == null)
                {
                    return Results.NotFound();
                }

                return TypedResults.Ok(boat);
            });

            app.MapDelete("/boat/{id}", async (MongoDBContext context,
               [FromRoute] string id,
               CancellationToken cancellationToken = default) =>
            {
                var boat = await context.Boats.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (boat == null)
                {
                    return Results.NotFound();
                }

                context.Boats.Remove(boat);
                await context.SaveChangesAsync();

                return TypedResults.Ok();
            });

            app.MapPut("/boat/{id}", async (MongoDBContext context,
                [FromRoute] string id,
                [FromBody] Boat boat,
                CancellationToken cancellationToken = default) =>
            {
                var boatActual = await context.Boats.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (boat == null)
                {
                    return Results.NotFound();
                }

                boatActual!.Name = boat.Name;
                boatActual.Size = boat.Size;
                boatActual.License = boat.License;

                await context.SaveChangesAsync();
                return TypedResults.NoContent();
            });

            async Task<IResult> GetBoat(MongoDBContext context, 
                string boatName)
            {
                var boat = await context.Boats.FirstOrDefaultAsync(x => x.Name == boatName);

                if (boat == null)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.Ok();
            }
        }
    }
}
