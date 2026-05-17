using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedPipPlanRepository : IPipPlanRepository
{
    private readonly IPipPlanRepository _innerRepository;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration;
    private const string CacheKeyPrefix = "PipPlan";

    public CachedPipPlanRepository(IPipPlanRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
    {
        _innerRepository = innerRepository;
        _cache = cache;
        _cacheDuration = cacheDuration;
    }

    public async Task<PipPlan?> GetByIdAsync(int pipId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}_GetById_{pipId}";
        if (!_cache.TryGetValue(key, out PipPlan? entity))
        {
            entity = await _innerRepository.GetByIdAsync(pipId, ct);
            if (entity != null)
            {
                _cache.Set(key, entity, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
            }
        }
        return entity;
    }

    public async Task<PipPlan?> GetByIdWithDetailsAsync(int pipId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}_GetByIdWithDetails_{pipId}";
        if (!_cache.TryGetValue(key, out PipPlan? entity))
        {
            entity = await _innerRepository.GetByIdWithDetailsAsync(pipId, ct);
            if (entity != null)
            {
                _cache.Set(key, entity, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
            }
        }
        return entity;
    }

    public async Task<IEnumerable<PipPlan>> GetAllAsync(CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}_GetAll";
        if (!_cache.TryGetValue(key, out IEnumerable<PipPlan>? entities))
        {
            entities = await _innerRepository.GetAllAsync(ct);
            _cache.Set(key, entities, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return entities ?? new List<PipPlan>();
    }

    public async Task<IEnumerable<PipPlan>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}_GetByEmployeeId_{employeeId}";
        if (!_cache.TryGetValue(key, out IEnumerable<PipPlan>? entities))
        {
            entities = await _innerRepository.GetByEmployeeIdAsync(employeeId, ct);
            _cache.Set(key, entities, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return entities ?? new List<PipPlan>();
    }

    public async Task<IEnumerable<PipPlan>> GetByManagerIdAsync(int managerId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}_GetByManagerId_{managerId}";
        if (!_cache.TryGetValue(key, out IEnumerable<PipPlan>? entities))
        {
            entities = await _innerRepository.GetByManagerIdAsync(managerId, ct);
            _cache.Set(key, entities, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return entities ?? new List<PipPlan>();
    }

    public async Task<bool> HasActivePipAsync(int employeeId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}_HasActivePip_{employeeId}";
        if (!_cache.TryGetValue(key, out bool hasActive))
        {
            hasActive = await _innerRepository.HasActivePipAsync(employeeId, ct);
            _cache.Set(key, hasActive, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return hasActive;
    }

    public async Task<PipPlan> CreateAsync(PipPlan plan, CancellationToken ct = default)
    {
        var result = await _innerRepository.CreateAsync(plan, ct);
        InvalidateCache();
        return result;
    }

    public async Task<PipPlan> UpdateAsync(PipPlan plan, CancellationToken ct = default)
    {
        var result = await _innerRepository.UpdateAsync(plan, ct);
        InvalidateCache();
        InvalidateItemCache(plan.Pipid);
        return result;
    }

    public async Task<bool> DeleteAsync(int pipId, CancellationToken ct = default)
    {
        var result = await _innerRepository.DeleteAsync(pipId, ct);
        if (result)
        {
            InvalidateCache();
            InvalidateItemCache(pipId);
        }
        return result;
    }

    private void InvalidateCache()
    {
        _cache.Remove($"{CacheKeyPrefix}_GetAll");
    }

    private void InvalidateItemCache(int pipId)
    {
        _cache.Remove($"{CacheKeyPrefix}_GetById_{pipId}");
        _cache.Remove($"{CacheKeyPrefix}_GetByIdWithDetails_{pipId}");
    }
}
