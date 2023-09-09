using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using EddBotBackend.Shared;
using EddBotBackend.Shared.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EddBotBackend.EndPoints;

public static class CategoryEndpoints
{
    public static void MapMkCatReqModelEndpoints (this IEndpointRouteBuilder routes, DiscordBotService discordBotService)
    {
        var group = routes.MapGroup("/api/Category").WithTags(nameof(MkCatReqModel));

        group.MapPost ("/", async Task (MkCatReqModel model) =>
        {           
            await discordBotService.CreateCategory(model.Category);
        })
        .WithName("CreateCategory")
        .WithOpenApi();
    }
}
