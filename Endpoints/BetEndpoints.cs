using Microsoft.AspNetCore.Mvc;
using WillTheyDie.Api.DTOs.Bets;
using WillTheyDie.Api.Services;
using System.Security.Claims;

namespace WillTheyDie.Api.Endpoints;

public static class BetEndpoints
{
    public static void MapBetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bets").WithTags("Bets");

        group.MapPost("/", async ([FromBody] PlaceBetRequest request, ClaimsPrincipal user, IBetService betService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await betService.PlaceBetAsync(userId, request);
            
            if (result == null)
            {
                return Results.BadRequest(new { message = "Unable to place bet. Check episode status, character status, and balance." });
            }

            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("PlaceBet")
        .WithOpenApi();

        group.MapGet("/me", async ([FromQuery] int? episodeId, ClaimsPrincipal user, IBetService betService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            var bets = await betService.GetUserBetsAsync(userId, episodeId);
            return Results.Ok(bets);
        })
        .RequireAuthorization()
        .WithName("GetMyBets")
        .WithOpenApi();
    }
}
