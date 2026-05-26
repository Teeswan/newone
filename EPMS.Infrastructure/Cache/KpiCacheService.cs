using System;
using System.Text.Json;
using System.Threading.Tasks;
using EPMS.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace EPMS.Infrastructure.Cache;

public class KpiCacheService : IKpiCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<KpiCacheService> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public KpiCacheService(IDistributedCache cache, ILogger<KpiCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(key);
            if (cachedData == null)
            {
                _logger.LogDebug("Cache MISS for key: {Key}", key);
                return default;
            }

            _logger.LogDebug("Cache HIT for key: {Key}", key);
            return JsonSerializer.Deserialize<T>(cachedData, _jsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize cache data for key: {Key}, treating as cache miss", key);
            await _cache.RemoveAsync(key);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accessing cache for key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            };

            var jsonData = JsonSerializer.Serialize(value, _jsonOptions);
            await _cache.SetStringAsync(key, jsonData, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set cache for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        // Redis pattern removal usually requires IConnectionMultiplexer
        // For IDistributedCache, we might need a more specialized implementation
        // or just skip it if it's too complex for this simplified version.
        // In a real scenario, we'd use StackExchange.Redis directly.
        _logger.LogWarning("RemoveByPatternAsync not fully implemented for IDistributedCache abstraction.");
        return Task.CompletedTask;
    }
}
