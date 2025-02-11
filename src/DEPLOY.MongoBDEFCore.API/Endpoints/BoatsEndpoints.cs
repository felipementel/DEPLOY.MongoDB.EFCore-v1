using Asp.Versioning;
using DEPLOY.MongoBDEFCore.API.Domain;
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


            app.MapPost("/boat", async (MongoDBContext context, Boat boat) =>
            {
                context.Boats.Add(new Boat { Name = boat.Name, Size = boat.Size, License = true });
                await context.SaveChangesAsync();

                return TypedResults.Created();
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

            app.MapDelete("/boat/{boatName}", async (MongoDBContext context, string boatName) =>
            {
                var boat = await context.Boats.FirstOrDefaultAsync(x => x.Name == boatName);
                context.Boats.Remove(boat);
                await context.SaveChangesAsync();

                return TypedResults.Ok();
            });

            app.MapPut("/boat/{boatName}", async (
                MongoDBContext context,
                string boatName,
                CancellationToken cancellationToken = default) =>
            {
                var boat = await context.Boats.FirstOrDefaultAsync(x => x.Name == boatName);

                if (boat == null)
                {
                    return Results.NotFound();
                }

                boat.License = false;
                await context.SaveChangesAsync();
                return TypedResults.Ok();
            });

            async Task<IResult> GetBoat(MongoDBContext context, string boarName)
            {
                var boat = await context.Boats.FirstOrDefaultAsync(x => x.Name == boarName);

                if (boat == null)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.Ok();
            }
        }
    }
}
