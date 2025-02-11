using Asp.Versioning;
using DEPLOY.MongoBDEFCore.API.Domain;
using DEPLOY.MongoBDEFCore.API.Infra.Database.Persistence;
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


        }
    }
}
