using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedMeetingNoteRepository : IMeetingNoteRepository
{
    private readonly IMeetingNoteRepository _innerRepository;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration;
    private const string CacheKeyPrefix = "MeetingNote";

    public CachedMeetingNoteRepository(IMeetingNoteRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
    {
        _innerRepository = innerRepository;
        _cache = cache;
        _cacheDuration = cacheDuration;
    }

    public async Task<IEnumerable<MeetingNote>> GetByMeetingIdAsync(int meetingId, CancellationToken cancellationToken)
    {
        string key = $"{CacheKeyPrefix}_GetByMeetingId_{meetingId}";
        if (!_cache.TryGetValue(key, out IEnumerable<MeetingNote>? entities))
        {
            entities = await _innerRepository.GetByMeetingIdAsync(meetingId, cancellationToken);
            _cache.Set(key, entities, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return entities ?? new List<MeetingNote>();
    }

    public async Task AddAsync(MeetingNote meetingNote, CancellationToken cancellationToken)
    {
        await _innerRepository.AddAsync(meetingNote, cancellationToken);
        InvalidateCache(meetingNote.MeetingId);
    }

    private void InvalidateCache(int meetingId)
    {
        _cache.Remove($"{CacheKeyPrefix}_GetByMeetingId_{meetingId}");
    }
}
