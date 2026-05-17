using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedMeetingRepository : IMeetingRepository
{
    private readonly IMeetingRepository _innerRepository;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration;
    private const string CacheKeyPrefix = "Meeting";

    public CachedMeetingRepository(IMeetingRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
    {
        _innerRepository = innerRepository;
        _cache = cache;
        _cacheDuration = cacheDuration;
    }

    public async Task<OneOnOneMeeting?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        string key = $"{CacheKeyPrefix}_GetById_{id}";
        if (!_cache.TryGetValue(key, out OneOnOneMeeting? entity))
        {
            entity = await _innerRepository.GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                _cache.Set(key, entity, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
            }
        }
        return entity;
    }

    public async Task<IEnumerable<OneOnOneMeeting>> GetHistoryTimelineAsync(int rootMeetingId, CancellationToken cancellationToken)
    {
        string key = $"{CacheKeyPrefix}_GetHistoryTimeline_{rootMeetingId}";
        if (!_cache.TryGetValue(key, out IEnumerable<OneOnOneMeeting>? entities))
        {
            entities = await _innerRepository.GetHistoryTimelineAsync(rootMeetingId, cancellationToken);
            _cache.Set(key, entities, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return entities ?? new List<OneOnOneMeeting>();
    }

    public async Task<IEnumerable<OneOnOneMeeting>> GetManagerDashboardAsync(int managerId, CancellationToken cancellationToken)
    {
        string key = $"{CacheKeyPrefix}_GetManagerDashboard_{managerId}";
        if (!_cache.TryGetValue(key, out IEnumerable<OneOnOneMeeting>? entities))
        {
            entities = await _innerRepository.GetManagerDashboardAsync(managerId, cancellationToken);
            _cache.Set(key, entities, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return entities ?? new List<OneOnOneMeeting>();
    }

    public async Task<IEnumerable<OneOnOneMeeting>> GetEmployeeDashboardAsync(int employeeId, CancellationToken cancellationToken)
    {
        string key = $"{CacheKeyPrefix}_GetEmployeeDashboard_{employeeId}";
        if (!_cache.TryGetValue(key, out IEnumerable<OneOnOneMeeting>? entities))
        {
            entities = await _innerRepository.GetEmployeeDashboardAsync(employeeId, cancellationToken);
            _cache.Set(key, entities, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return entities ?? new List<OneOnOneMeeting>();
    }

    public async Task AddAsync(OneOnOneMeeting meeting, CancellationToken cancellationToken)
    {
        await _innerRepository.AddAsync(meeting, cancellationToken);
        InvalidateCache();
    }

    private void InvalidateCache()
    {
        // Invalidate all meeting cache keys
        _cache.Remove($"{CacheKeyPrefix}_GetById_");
        _cache.Remove($"{CacheKeyPrefix}_GetHistoryTimeline_");
        _cache.Remove($"{CacheKeyPrefix}_GetManagerDashboard_");
        _cache.Remove($"{CacheKeyPrefix}_GetEmployeeDashboard_");
    }
}
