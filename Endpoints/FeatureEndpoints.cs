using Microsoft.FeatureManagement;
using WillTheyDie.Api.Configuration;

namespace WillTheyDie.Api.Endpoints;

public static class FeatureEndpoints
{
    public static void MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/features")
            .WithTags("Features");

        group.MapGet("/", async (IFeatureManager featureManager) =>
        {
            var features = new Dictionary<string, bool>
            {
                { FeatureFlags.BettingEnabled, await featureManager.IsEnabledAsync(FeatureFlags.BettingEnabled) },
                { FeatureFlags.LeaderboardEnabled, await featureManager.IsEnabledAsync(FeatureFlags.LeaderboardEnabled) },
                { FeatureFlags.RealTimeBetting, await featureManager.IsEnabledAsync(FeatureFlags.RealTimeBetting) },
                { FeatureFlags.SocialSharing, await featureManager.IsEnabledAsync(FeatureFlags.SocialSharing) },
                { FeatureFlags.BetRecommendations, await featureManager.IsEnabledAsync(FeatureFlags.BetRecommendations) }
            };

            return Results.Ok(features);
        })
        .WithName("GetFeatureFlags")
        .WithDescription("Get status of all feature flags")
        .Produces<Dictionary<string, bool>>(StatusCodes.Status200OK);

        group.MapGet("/{featureName}", async (string featureName, IFeatureManager featureManager) =>
        {
            var isEnabled = await featureManager.IsEnabledAsync(featureName);
            return Results.Ok(new { feature = featureName, enabled = isEnabled });
        })
        .WithName("GetFeatureFlag")
        .WithDescription("Get status of a specific feature flag")
        .Produces<object>(StatusCodes.Status200OK);
    }
}
