using Microsoft.AspNetCore.Mvc;
using WillTheyDie.Api.Services;
using System.Security.Claims;

namespace WillTheyDie.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/me/shows/{showId:int}/balance", async (int showId, ClaimsPrincipal user, IShowService showService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            var balance = await showService.GetUserBalanceAsync(userId, showId);
            
            if (balance == null)
            {
                return Results.NotFound(new { message = "User has not joined this show" });
            }

            return Results.Ok(new { balance });
        })
        .RequireAuthorization()
        .WithName("GetUserShowBalance")
        .WithOpenApi();
    }
}
