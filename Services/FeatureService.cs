using Microsoft.FeatureManagement;
using WillTheyDie.Api.Configuration;

namespace WillTheyDie.Api.Services;

public interface IFeatureService
{
    Task<bool> IsBettingEnabledAsync();
    Task<bool> IsLeaderboardEnabledAsync();
    Task<bool> IsRealTimeBettingEnabledAsync();
    Task<bool> IsSocialSharingEnabledAsync();
    Task<bool> AreBetRecommendationsEnabledAsync();
    Task<Dictionary<string, bool>> GetAllFeaturesAsync();
}

public class FeatureService : IFeatureService
{
    private readonly IFeatureManager _featureManager;
    private readonly ILogger<FeatureService> _logger;

    public FeatureService(IFeatureManager featureManager, ILogger<FeatureService> logger)
    {
        _featureManager = featureManager;
        _logger = logger;
    }

    public async Task<bool> IsBettingEnabledAsync()
    {
        var isEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.BettingEnabled);
        _logger.LogDebug("Feature {FeatureName} is {Status}", FeatureFlags.BettingEnabled, isEnabled ? "enabled" : "disabled");
        return isEnabled;
    }

    public async Task<bool> IsLeaderboardEnabledAsync()
    {
        var isEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.LeaderboardEnabled);
        _logger.LogDebug("Feature {FeatureName} is {Status}", FeatureFlags.LeaderboardEnabled, isEnabled ? "enabled" : "disabled");
        return isEnabled;
    }

    public async Task<bool> IsRealTimeBettingEnabledAsync()
    {
        var isEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.RealTimeBetting);
        _logger.LogDebug("Feature {FeatureName} is {Status}", FeatureFlags.RealTimeBetting, isEnabled ? "enabled" : "disabled");
        return isEnabled;
    }

    public async Task<bool> IsSocialSharingEnabledAsync()
    {
        var isEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.SocialSharing);
        _logger.LogDebug("Feature {FeatureName} is {Status}", FeatureFlags.SocialSharing, isEnabled ? "enabled" : "disabled");
        return isEnabled;
    }

    public async Task<bool> AreBetRecommendationsEnabledAsync()
    {
        var isEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.BetRecommendations);
        _logger.LogDebug("Feature {FeatureName} is {Status}", FeatureFlags.BetRecommendations, isEnabled ? "enabled" : "disabled");
        return isEnabled;
    }

    public async Task<Dictionary<string, bool>> GetAllFeaturesAsync()
    {
        return new Dictionary<string, bool>
        {
            { FeatureFlags.BettingEnabled, await IsBettingEnabledAsync() },
            { FeatureFlags.LeaderboardEnabled, await IsLeaderboardEnabledAsync() },
            { FeatureFlags.RealTimeBetting, await IsRealTimeBettingEnabledAsync() },
            { FeatureFlags.SocialSharing, await IsSocialSharingEnabledAsync() },
            { FeatureFlags.BetRecommendations, await AreBetRecommendationsEnabledAsync() }
        };
    }
}
