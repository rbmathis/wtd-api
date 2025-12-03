# Phase 2 Implementation: Azure App Configuration & Feature Flags

## ✅ Implementation Summary

Phase 2 of the feature plan has been successfully implemented. This phase adds Azure App Configuration integration and Feature Management capabilities to the WillTheyDie API.

## 📦 Packages Installed

- ✅ Azure.Identity (v1.13.0)
- ✅ Microsoft.Extensions.Configuration.AzureAppConfiguration (v8.0.0)
- ✅ Microsoft.FeatureManagement.AspNetCore (v4.0.0)

## 🏗️ Architecture Changes

### New Files Created

1. **Configuration/AzureAppConfigSettings.cs**
   - Configuration model for Azure App Configuration settings
   - Controls connection string, enabled state, refresh interval, and watched keys

2. **Services/FeatureService.cs**
   - Service interface and implementation for feature flag management
   - Provides typed methods for checking each feature flag
   - Includes logging for debugging feature flag state

3. **Endpoints/FeatureEndpoints.cs**
   - REST API endpoints for querying feature flag status
   - `GET /api/features` - Returns all feature flags and their states
   - `GET /api/features/{featureName}` - Returns specific feature flag state

### Modified Files

1. **Program.cs**
   - Added Azure App Configuration integration with dynamic refresh
   - Registered Feature Management services
   - Added FeatureService to dependency injection
   - Registered feature endpoints

2. **appsettings.json**
   - Added `AzureAppConfiguration` section
   - Added `FeatureManagement` section with default feature flags

3. **appsettings.Development.json**
   - Added development-specific feature flag configuration
   - All features enabled by default in development

## 🎯 Feature Flags Available

The following feature flags are configured and ready to use:

- **BettingEnabled** - Controls whether users can place bets
- **LeaderboardEnabled** - Controls leaderboard visibility
- **RealTimeBetting** - Controls real-time betting features
- **SocialSharing** - Controls social sharing features
- **BetRecommendations** - Controls ML-powered bet recommendations

## 🚀 Usage Examples

### 1. Local Configuration (No Azure Required)

Feature flags work immediately using local configuration in `appsettings.json`:

```json
{
  "FeatureManagement": {
    "BettingEnabled": true,
    "LeaderboardEnabled": true,
    "RealTimeBetting": false,
    "SocialSharing": false,
    "BetRecommendations": false
  }
}
```

### 2. Using Feature Flags in Code

**Option A: Via IFeatureService (Recommended)**

```csharp
public class MyService
{
    private readonly IFeatureService _featureService;
    
    public MyService(IFeatureService featureService)
    {
        _featureService = featureService;
    }
    
    public async Task DoSomethingAsync()
    {
        if (await _featureService.IsBettingEnabledAsync())
        {
            // Betting logic here
        }
    }
}
```

**Option B: Via IFeatureManager (Direct)**

```csharp
public class MyController
{
    private readonly IFeatureManager _featureManager;
    
    public MyController(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }
    
    public async Task<IResult> PlaceBet()
    {
        if (!await _featureManager.IsEnabledAsync(FeatureFlags.BettingEnabled))
        {
            return Results.BadRequest("Betting is currently disabled");
        }
        
        // Place bet logic
        return Results.Ok();
    }
}
```

### 3. Using Feature Flags in Endpoints

```csharp
app.MapPost("/api/bets", async (IFeatureManager featureManager, BetDto bet) =>
{
    if (!await featureManager.IsEnabledAsync(FeatureFlags.BettingEnabled))
    {
        return Results.Problem("Betting is currently disabled", statusCode: 503);
    }
    
    // Process bet
    return Results.Ok();
})
.RequireAuthorization();
```

### 4. Checking Feature Status via API

```bash
# Get all feature flags
curl http://localhost:5000/api/features

# Response:
{
  "BettingEnabled": true,
  "LeaderboardEnabled": true,
  "RealTimeBetting": false,
  "SocialSharing": false,
  "BetRecommendations": false
}

# Get specific feature flag
curl http://localhost:5000/api/features/BettingEnabled

# Response:
{
  "feature": "BettingEnabled",
  "enabled": true
}
```

## 🔧 Azure App Configuration Setup (Optional)

To use Azure App Configuration for centralized, dynamic feature management:

### Step 1: Create Azure App Configuration Resource

```bash
# Create resource group
az group create --name rg-willtheydie --location eastus

# Create App Configuration store
az appconfig create \
  --name appconfig-willtheydie \
  --resource-group rg-willtheydie \
  --location eastus \
  --sku Standard
```

### Step 2: Get Connection String

```bash
az appconfig credential list \
  --name appconfig-willtheydie \
  --resource-group rg-willtheydie \
  --query "[?name=='Primary'].connectionString" -o tsv
```

### Step 3: Update appsettings.json

```json
{
  "AzureAppConfiguration": {
    "ConnectionString": "Endpoint=https://appconfig-willtheydie.azconfig.io;Id=xxx;Secret=xxx",
    "Enabled": true,
    "RefreshIntervalSeconds": 30,
    "WatchedKeys": [ "WillTheyDie:*" ]
  }
}
```

### Step 4: Add Feature Flags in Azure Portal

1. Go to Azure Portal → App Configuration → Feature Manager
2. Create feature flags:
   - `BettingEnabled`
   - `LeaderboardEnabled`
   - `RealTimeBetting`
   - `SocialSharing`
   - `BetRecommendations`

### Step 5: Advanced Feature Filters (Optional)

You can configure advanced filters in Azure App Configuration:

**Percentage Rollout:**
```json
{
  "id": "RealTimeBetting",
  "enabled": true,
  "conditions": {
    "client_filters": [
      {
        "name": "Microsoft.Percentage",
        "parameters": {
          "Value": 25
        }
      }
    ]
  }
}
```

**Time Window:**
```json
{
  "id": "LeaderboardEnabled",
  "enabled": true,
  "conditions": {
    "client_filters": [
      {
        "name": "Microsoft.TimeWindow",
        "parameters": {
          "Start": "2024-01-01T00:00:00Z",
          "End": "2024-12-31T23:59:59Z"
        }
      }
    ]
  }
}
```

## 🔄 Dynamic Refresh

When Azure App Configuration is enabled:
- Feature flags are automatically refreshed every 30 seconds (configurable)
- No application restart required for feature flag changes
- Changes propagate to all running instances

## 🧪 Testing

### Test 1: Verify Build
```bash
dotnet build WillTheyDie.Api.csproj
# Should succeed without errors
```

### Test 2: Run Application
```bash
dotnet run --project WillTheyDie.Api.csproj
# Application should start successfully
```

### Test 3: Check Feature Flags
```bash
curl http://localhost:5000/api/features
# Should return all feature flags with their states
```

### Test 4: Toggle Feature in appsettings.json
1. Change `BettingEnabled` to `false` in appsettings.json
2. Restart application
3. Check `/api/features` - should show `BettingEnabled: false`

## 📊 Benefits Delivered

✅ **Zero-downtime deployments** - Toggle features without redeploying  
✅ **A/B testing ready** - Percentage-based rollouts via Azure App Config  
✅ **Environment isolation** - Different features enabled per environment  
✅ **Gradual rollouts** - Roll out features to small percentage of users  
✅ **Quick rollback** - Disable problematic features instantly  
✅ **Centralized management** - Manage all feature flags from Azure Portal  

## 🔐 Security Considerations

1. **Connection Strings** - Store Azure App Config connection string in:
   - Azure Key Vault (production)
   - User Secrets (development): `dotnet user-secrets set "AzureAppConfiguration:ConnectionString" "your-connection-string"`
   - Environment variables (CI/CD)

2. **Managed Identity** - For production, use Managed Identity instead of connection strings:

```csharp
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri("https://appconfig-willtheydie.azconfig.io"), 
                    new DefaultAzureCredential())
           .ConfigureRefresh(...)
           .UseFeatureFlags(...);
});
```

## 📝 Next Steps

Phase 2 is complete! The following capabilities are now available:

- ✅ Feature flag infrastructure
- ✅ Local configuration support
- ✅ Azure App Configuration integration (ready to enable)
- ✅ Dynamic refresh capabilities
- ✅ Feature flag API endpoints
- ✅ Type-safe feature service

You can now:
1. Use feature flags in your code to control features
2. Enable Azure App Configuration by setting connection string and `Enabled: true`
3. Configure advanced filters (percentage, time window, targeting)
4. Monitor feature usage and toggle features in real-time

## 🆘 Troubleshooting

### Issue: Features not refreshing
**Solution:** Check `RefreshIntervalSeconds` in configuration. The refresh only happens when the app is actively processing requests.

### Issue: Azure App Config connection fails
**Solution:** Verify connection string is correct and the App Configuration resource exists. Check network connectivity.

### Issue: Feature flag not found
**Solution:** Ensure feature flag name exactly matches the constant in `FeatureFlags.cs` and is defined in configuration.

---

**Implementation Date:** December 3, 2025  
**Status:** ✅ Complete  
**Build Status:** ✅ Passing  
