using Microsoft.AspNetCore.Mvc;
using WillTheyDie.Api.Services;
using System.Security.Claims;

namespace WillTheyDie.Api.Endpoints;

public static class ShowEndpoints
{
    public static void MapShowEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/shows").WithTags("Shows");

        group.MapGet("/", async (IShowService showService) =>
        {
            var shows = await showService.GetAllShowsAsync();
            return Results.Ok(shows);
        })
        .WithName("GetAllShows")
        .WithOpenApi();

        group.MapGet("/{id:int}", async (int id, IShowService showService) =>
        {
            var show = await showService.GetShowDetailAsync(id);
            
            if (show == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(show);
        })
        .WithName("GetShowDetail")
        .WithOpenApi();

        group.MapPost("/{id:int}/join", async (int id, ClaimsPrincipal user, IShowService showService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            var success = await showService.JoinShowAsync(userId, id);
            
            if (!success)
            {
                return Results.BadRequest(new { message = "Unable to join show. You may already be a member." });
            }

            return Results.Ok(new { message = "Successfully joined show" });
        })
        .RequireAuthorization()
        .WithName("JoinShow")
        .WithOpenApi();

        group.MapGet("/{id:int}/characters", async (int id, [FromQuery] bool? aliveOnly, IShowService showService) =>
        {
            var characters = await showService.GetShowCharactersAsync(id, aliveOnly);
            return Results.Ok(characters);
        })
        .WithName("GetShowCharacters")
        .WithOpenApi();

        group.MapGet("/{id:int}/leaderboard", async (int id, IShowService showService, [FromQuery] int limit = 10) =>
        {
            var leaderboard = await showService.GetLeaderboardAsync(id, limit);
            return Results.Ok(leaderboard);
        })
        .WithName("GetShowLeaderboard")
        .WithOpenApi();
    }
}
