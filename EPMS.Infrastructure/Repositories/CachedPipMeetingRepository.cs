using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedPipMeetingRepository : IPipMeetingRepository
{
    private readonly IPipMeetingRepository _innerRepository;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration;
    private const string CacheKeyPrefix = "PipMeeting";

    public CachedPipMeetingRepository(IPipMeetingRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
    {
        _innerRepository = innerRepository;
        _cache = cache;
        _cacheDuration = cacheDuration;
    }

    public async Task<PipMeeting?> GetByIdAsync(int meetingId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}_GetById_{meetingId}";
        if (!_cache.TryGetValue(key, out PipMeeting? entity))
        {
            entity = await _innerRepository.GetByIdAsync(meetingId, ct);
            if (entity != null)
            {
                _cache.Set(key, entity, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
            }
        }
        return entity;
    }

    public async Task<IEnumerable<PipMeeting>> GetByPipIdAsync(int pipId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}_GetByPipId_{pipId}";
        if (!_cache.TryGetValue(key, out IEnumerable<PipMeeting>? entities))
        {
            entities = await _innerRepository.GetByPipIdAsync(pipId, ct);
            _cache.Set(key, entities, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return entities ?? new List<PipMeeting>();
    }

    public async Task<PipMeeting?> GetLatestMeetingAsync(int pipId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}_GetLatestMeeting_{pipId}";
        if (!_cache.TryGetValue(key, out PipMeeting? entity))
        {
            entity = await _innerRepository.GetLatestMeetingAsync(pipId, ct);
            if (entity != null)
            {
                _cache.Set(key, entity, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
            }
        }
        return entity;
    }

    public async Task<PipMeeting> CreateAsync(PipMeeting meeting, CancellationToken ct = default)
    {
        var result = await _innerRepository.CreateAsync(meeting, ct);
        InvalidateCache();
        return result;
    }

    public async Task<PipMeeting> UpdateAsync(PipMeeting meeting, CancellationToken ct = default)
    {
        var result = await _innerRepository.UpdateAsync(meeting, ct);
        InvalidateCache();
        InvalidateItemCache(meeting.PipMeetingId);
        return result;
    }

    public async Task<bool> DeleteAsync(int meetingId, CancellationToken ct = default)
    {
        var result = await _innerRepository.DeleteAsync(meetingId, ct);
        if (result)
        {
            InvalidateCache();
            InvalidateItemCache(meetingId);
        }
        return result;
    }

    private void InvalidateCache()
    {
    }

    private void InvalidateItemCache(int meetingId)
    {
        _cache.Remove($"{CacheKeyPrefix}_GetById_{meetingId}");
    }
}
