using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using WillTheyDie.Api.Configuration;
using Microsoft.Extensions.Options;

namespace WillTheyDie.Api.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly RedisSettings _settings;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IDistributedCache cache, 
        IOptions<RedisSettings> settings,
        ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (!_settings.Enabled) return null;

        try
        {
            var fullKey = $"{_settings.InstanceName}{key}";
            var cachedData = await _cache.GetStringAsync(fullKey, cancellationToken);

            if (string.IsNullOrEmpty(cachedData))
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return null;
            }

            _logger.LogDebug("Cache hit for key: {Key}", key);
            return JsonSerializer.Deserialize<T>(cachedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        if (!_settings.Enabled) return;

        try
        {
            var fullKey = $"{_settings.InstanceName}{key}";
            var serializedData = JsonSerializer.Serialize(value);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(_settings.DefaultExpirationMinutes)
            };

            await _cache.SetStringAsync(fullKey, serializedData, options, cancellationToken);
            _logger.LogDebug("Cache set for key: {Key}, expiration: {Expiration} minutes", 
                key, (expiration ?? TimeSpan.FromMinutes(_settings.DefaultExpirationMinutes)).TotalMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled) return;

        try
        {
            var fullKey = $"{_settings.InstanceName}{key}";
            await _cache.RemoveAsync(fullKey, cancellationToken);
            _logger.LogDebug("Cache removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key: {Key}", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled) return;

        _logger.LogWarning("RemoveByPrefixAsync is not supported by IDistributedCache. Consider using a Redis-specific implementation.");
        await Task.CompletedTask;
    }
}
