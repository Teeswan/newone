using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedPipObjectiveRepository : IPipObjectiveRepository
{
    private readonly IPipObjectiveRepository _innerRepository;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration;
    private const string CacheKeyPrefix = "PipObjective";

    public CachedPipObjectiveRepository(IPipObjectiveRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
    {
        _innerRepository = innerRepository;
        _cache = cache;
        _cacheDuration = cacheDuration;
    }

    public async Task<PipObjective?> GetByIdAsync(int objectiveId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}_GetById_{objectiveId}";
        if (!_cache.TryGetValue(key, out PipObjective? entity))
        {
            entity = await _innerRepository.GetByIdAsync(objectiveId, ct);
            if (entity != null)
            {
                _cache.Set(key, entity, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
            }
        }
        return entity;
    }

    public async Task<IEnumerable<PipObjective>> GetByPipIdAsync(int pipId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}_GetByPipId_{pipId}";
        if (!_cache.TryGetValue(key, out IEnumerable<PipObjective>? entities))
        {
            entities = await _innerRepository.GetByPipIdAsync(pipId, ct);
            _cache.Set(key, entities, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return entities ?? new List<PipObjective>();
    }

    public async Task<int> GetAchievedCountByPipIdAsync(int pipId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}_GetAchievedCountByPipId_{pipId}";
        if (!_cache.TryGetValue(key, out int count))
        {
            count = await _innerRepository.GetAchievedCountByPipIdAsync(pipId, ct);
            _cache.Set(key, count, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return count;
    }

    public async Task<PipObjective> CreateAsync(PipObjective objective, CancellationToken ct = default)
    {
        var result = await _innerRepository.CreateAsync(objective, ct);
        InvalidateCache();
        return result;
    }

    public async Task<PipObjective> UpdateAsync(PipObjective objective, CancellationToken ct = default)
    {
        var result = await _innerRepository.UpdateAsync(objective, ct);
        InvalidateCache();
        InvalidateItemCache(objective.ObjectiveId);
        return result;
    }

    public async Task<bool> DeleteAsync(int objectiveId, CancellationToken ct = default)
    {
        var result = await _innerRepository.DeleteAsync(objectiveId, ct);
        if (result)
        {
            InvalidateCache();
            InvalidateItemCache(objectiveId);
        }
        return result;
    }

    private void InvalidateCache()
    {
    }

    private void InvalidateItemCache(int objectiveId)
    {
        _cache.Remove($"{CacheKeyPrefix}_GetById_{objectiveId}");
    }
}
