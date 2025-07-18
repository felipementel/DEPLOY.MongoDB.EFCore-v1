using Asp.Versioning;
using DEPLOY.MongoBDEFCore.API.Domain;
using DEPLOY.MongoBDEFCore.API.Infra.Database.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DEPLOY.MongoBDEFCore.API.Endpoints
{
    public static class MarinasEndpoints
    {
        public static void MapMarinasEndpoints(this IEndpointRouteBuilder app)
        {
            var apiVersionSetMarinas = app
                .NewApiVersionSet("marinas")
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var marinas = app
                .MapGroup("/api/v{apiVersion:apiVersion}/marinas")
                //.RequireAuthorization()
                .WithApiVersionSet(apiVersionSetMarinas);


            marinas
                .MapPost("/", async (MongoDBContext context,
                [FromBody] Marina marina) =>
                {
                    if (string.IsNullOrWhiteSpace(marina.Name))
                    {
                        return Results.UnprocessableEntity("Name is required");
                    }

                    context.Marinas.Add(new Marina { Name = marina.Name });
                    await context.SaveChangesAsync();

                    return TypedResults.Created($"/searchbyid/{marina.Id}", marina);
                })
                .Produces(201)
                .Produces(422)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "marina-post",
                    Summary = "create a marina",
                    Description = "process to register a new marina",
                    Tags = new List<OpenApiTag> { new() { Name = "marina" } }
                });

            marinas
                .MapGet("/", GetMarina)
                .Produces(201)
                .Produces(422)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "marina-get",
                    Summary = "get a marina",
                    Description = "process to get a marina",
                    Tags = new List<OpenApiTag> { new() { Name = "Marina" } }
                });

            marinas.MapGet("/all",
                async (MongoDBContext context) =>
                {
                    var items = await context.Marinas.ToListAsync();

                    if (items.Count == 0)
                    {
                        return Results.NotFound();
                    }

                    return TypedResults.Ok(items);
                })
                .Produces(200)
                .Produces(404)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "marina-all-get",
                    Summary = "get all marinas",
                    Description = "process to get all marinas",
                    Tags = new List<OpenApiTag> { new() { Name = "Marina" } }
                });

            marinas.MapGet("/{marinaName}",
                async (MongoDBContext context,
                [FromRoute] string marinaName,
                CancellationToken cancellationToken = default) =>
                {
                    var matchingBoats = await context.Marinas
                    .Where(x => x.Name.ToUpper().Contains(marinaName.ToUpper()))
                    .ToListAsync(cancellationToken);

                    if (matchingBoats.Count == 0)
                    {
                        return Results.NotFound();
                    }

                    return TypedResults.Ok(matchingBoats);
                })
                .Produces(200)
                .Produces(404)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "marina-by-name-get",
                    Summary = "get marina by name",
                    Description = "process to get marinas by name",
                    Tags = new List<OpenApiTag> { new() { Name = "Marinas" } }
                });

            marinas
                .MapGet("/searchbyid/{id}",
                async (MongoDBContext context,
                [FromRoute] string id,
                CancellationToken cancellationToken = default) =>
                {
                    var marina = await context.Marinas
                    .FirstOrDefaultAsync(x => x.Id == ObjectId.Parse(id), cancellationToken);

                    if (marina == null)
                    {
                        return Results.NotFound();
                    }

                    return TypedResults.Ok(marina);
                })
                .Produces(200)
                .Produces(404)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "marina-by-id-get",
                    Summary = "get marina by id",
                    Description = "process to get marinas by id",
                    Tags = new List<OpenApiTag> { new() { Name = "Marinas" } }
                });

            marinas
                .MapDelete("/{id}",
                async (MongoDBContext context,
                [FromRoute] string id,
                CancellationToken cancellationToken = default) =>
                {
                    var marina = await context.Marinas
                    .FirstOrDefaultAsync(x => x.Id == ObjectId.Parse(id), cancellationToken);

                    if (marina == null)
                    {
                        return Results.NotFound();
                    }

                    context.Marinas.Remove(marina);
                    await context.SaveChangesAsync();

                    return TypedResults.Ok();
                })
                .Produces(200)
                .Produces(404)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "marina-by-name-get",
                    Summary = "get marina by name",
                    Description = "process to get marinas by name",
                    Tags = new List<OpenApiTag> { new() { Name = "Marinas" } }
                });

            marinas.MapPut("/{id}",
                async (MongoDBContext context,
                [FromRoute] string id,
                [FromBody] Marina marina,
                CancellationToken cancellationToken = default) =>
                {
                    var marinaActual = await context.Marinas
                    .FirstOrDefaultAsync(x => x.Id == ObjectId.Parse(id), cancellationToken);

                    if (marina == null)
                    {
                        return Results.NotFound();
                    }

                    marinaActual!.Name = marina.Name;

                    await context.SaveChangesAsync();
                    return TypedResults.NoContent();
                })
                .Produces(204)
                .Produces(404)
                .Produces(500)
                .WithOpenApi(operation => new(operation)
                {
                    OperationId = "put-marina",
                    Summary = "update marina",
                    Description = "process to get marinas by name",
                    Tags = new List<OpenApiTag> { new() { Name = "Marinas" } }
                });

            async Task<IResult> GetMarina(MongoDBContext context,
                string marinaName)
            {
                var marina = await context.Marinas
                    .FirstOrDefaultAsync(x => x.Name == marinaName);

                if (marina == null)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.Ok();
            }
        }
    }
}
