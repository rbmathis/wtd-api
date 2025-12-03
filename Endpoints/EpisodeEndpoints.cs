using Microsoft.AspNetCore.Mvc;
using WillTheyDie.Api.Services;

namespace WillTheyDie.Api.Endpoints;

public static class EpisodeEndpoints
{
    public static void MapEpisodeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("Episodes");

        group.MapGet("/seasons/{seasonId:int}/episodes", async (int seasonId, IShowService showService) =>
        {
            var episodes = await showService.GetSeasonEpisodesAsync(seasonId);
            return Results.Ok(episodes);
        })
        .WithName("GetSeasonEpisodes")
        .WithOpenApi();

        group.MapGet("/episodes/{id:int}", async (int id, IShowService showService) =>
        {
            var episode = await showService.GetEpisodeDetailAsync(id);
            
            if (episode == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(episode);
        })
        .WithName("GetEpisodeDetail")
        .WithOpenApi();
    }
}
