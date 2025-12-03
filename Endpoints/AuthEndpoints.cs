using Microsoft.AspNetCore.Mvc;
using WillTheyDie.Api.DTOs.Auth;
using WillTheyDie.Api.Services;
using System.Security.Claims;

namespace WillTheyDie.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/register", async ([FromBody] RegisterRequest request, IAuthService authService) =>
        {
            var result = await authService.RegisterAsync(request);
            
            if (result == null)
            {
                return Results.BadRequest(new { message = "Username or email already exists" });
            }

            return Results.Ok(result);
        })
        .WithName("Register")
        .WithOpenApi();

        group.MapPost("/login", async ([FromBody] LoginRequest request, IAuthService authService) =>
        {
            var result = await authService.LoginAsync(request);
            
            if (result == null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(result);
        })
        .WithName("Login")
        .WithOpenApi();

        group.MapGet("/me", async (ClaimsPrincipal user, IAuthService authService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            var profile = await authService.GetUserProfileAsync(userId);
            
            if (profile == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(profile);
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser")
        .WithOpenApi();
    }
}
