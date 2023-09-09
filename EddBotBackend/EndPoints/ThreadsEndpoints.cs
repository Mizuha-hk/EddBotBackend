using EddBotBackend.Shared;
using EddBotBackend.Shared.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace EddBotBackend.EndPoints;

public static class ThreadsEndpoints
{
    public static void MapMkThreadsModelEndpoints (this IEndpointRouteBuilder routes, DiscordBotService discordBotService)
    {
        var group = routes.MapGroup("/api/Threads").WithTags(nameof(MkThreadsModel));

        group.MapPost("/", async (MkThreadsModel model) =>
        {
            var result = await discordBotService.CreateThreads(model.Category, model.Channel.ToLower(), model.Threads);

            if(result == DiscordBotService.CategoryStatus.CategoryNotFound)
            {
                return Results.NotFound("Category Not Found.");
            }
            else if(result == DiscordBotService.CategoryStatus.CreateChannelFailed)
            {
                return Results.Problem("Create Text Channel Failed.");
            }
            else
            {
                return Results.Ok();
            }
        })
        .WithName("CreateThreads")
        .WithOpenApi();

    }
}
