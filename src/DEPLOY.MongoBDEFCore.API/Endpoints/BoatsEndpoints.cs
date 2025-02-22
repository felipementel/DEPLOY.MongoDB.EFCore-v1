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
            var apiVersionSetBoats = app
                .NewApiVersionSet("boats")
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var boats = app
                .MapGroup("/api/v{version:apiVersion}/boats")
                //.RequireAuthorization()
                .WithApiVersionSet(apiVersionSetBoats);


            boats
                .MapPost("/",
                async (MongoDBContext context,
                [FromBody] Boat boat) =>
                {
                    if (string.IsNullOrWhiteSpace(boat.Name))
                    {
                        return Results.UnprocessableEntity("Name is required");
                    }

                    if (boat.Size <= 0)
                    {
                        return Results.UnprocessableEntity("Size is required");
                    }

                    var NewBoat = new Boat
                    {
                        Name = boat.Name,
                        Size = boat.Size,
                        License = true
                    };

                    context.Boats.Add(NewBoat);
                    await context.SaveChangesAsync();

                    return TypedResults.Created($"/searchbyid/{NewBoat.Id}", NewBoat);
                })
                .Produces(201)
                .Produces(422)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "boat-post",
                    Summary = "create a boat",
                    Description = "process do register a new boat",
                    Tags = new List<OpenApiTag> { new() { Name = "Boats" } }
                });

            boats
                .MapGet("/", GetBoat)
                .Produces(200)
                .Produces(422)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "boat-get",
                    Summary = "get a boat",
                    Description = "process do get a boat",
                    Tags = new List<OpenApiTag> { new() { Name = "Boats" } }
                });

            boats
                .MapGet("/all", async (MongoDBContext context) =>
                {
                    var items = await context.Boats.ToListAsync();

                    return TypedResults.Ok(items);
                })
                .Produces(200)
                .Produces(422)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "boat-get-all",
                    Summary = "get a boat",
                    Description = "process to get all boat",
                    Tags = new List<OpenApiTag> { new() { Name = "Boats" } }
                });

            boats
                .MapGet("/{boatName}", async (MongoDBContext context,
                [FromRoute] string boatName,
                CancellationToken cancellationToken = default) =>
                {
                    var matchingBoats = await context.Boats
                    .Where(x => x.Name.ToUpper().Contains(boatName.ToUpper()))
                    .ToListAsync(cancellationToken);

                    if (matchingBoats.Count == 0)
                    {
                        return Results.NotFound();
                    }

                    return TypedResults.Ok(matchingBoats);
                })
                .Produces(200)
                .Produces(422)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "boat-get",
                    Summary = "get a boat by name",
                    Description = "process to get a boat by name",
                    Tags = new List<OpenApiTag> { new() { Name = "Boats" } }
                });

            boats
                .MapGet("/searchbyid/{id}", async
                (MongoDBContext context,
                [FromRoute] string id,
                CancellationToken cancellationToken = default) =>
                {
                    var boat = await context.Boats
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                    if (boat == null)
                    {
                        return Results.NotFound();
                    }

                    return TypedResults.Ok(boat);
                })
                .Produces(200)
                .Produces(422)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "boat-get-by-id",
                    Summary = "get a boat by id",
                    Description = "process to get a boat by id",
                    Tags = new List<OpenApiTag> { new() { Name = "Boats" } }
                });

            boats
                .MapDelete("/{id}", async (MongoDBContext context,
                [FromRoute] string id,
                CancellationToken cancellationToken = default) =>
                {
                    var boat = await context.Boats
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                    if (boat == null)
                    {
                        return Results.NotFound();
                    }

                    context.Boats.Remove(boat);
                    await context.SaveChangesAsync();

                    return TypedResults.NoContent();
                })
                .Produces(200)
                .Produces(404)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "boat-delete",
                    Summary = "delete a boat",
                    Description = "process to delete a boat",
                    Tags = new List<OpenApiTag> { new() { Name = "Boats" } }
                });

            boats
                .MapPut("/{id}", async (MongoDBContext context,
                [FromRoute] string id,
                [FromBody] Boat boat,
                CancellationToken cancellationToken = default) =>
                {
                    var boatActual = await context.Boats
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                    if (boat == null)
                    {
                        return Results.NotFound();
                    }

                    boatActual!.Name = boat.Name;
                    boatActual.Size = boat.Size;
                    boatActual.License = boat.License;

                    await context.SaveChangesAsync();

                    return TypedResults.NoContent();
                })
                .Produces(204)
                .Produces(422)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "boat-put",
                    Summary = "update a boat",
                    Description = "process to update a boat",
                    Tags = new List<OpenApiTag> { new() { Name = "Boats" } }
                });

            async Task<IResult> GetBoat(MongoDBContext context,
                string boatName)
            {
                var boat = await context.Boats.FirstOrDefaultAsync(x => x.Name == boatName);

                if (boat == null)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.Ok(boat);
            }
        }
    }
}
