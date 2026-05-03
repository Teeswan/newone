using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedPositionPermissionRepository : IPositionPermissionRepository
{
    private readonly IPositionPermissionRepository _innerRepository;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration;
    private readonly string _cacheKeyPrefix = nameof(PositionPermission);

    public CachedPositionPermissionRepository(IPositionPermissionRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
    {
        _innerRepository = innerRepository ?? throw new ArgumentNullException(nameof(innerRepository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheDuration = cacheDuration;
    }

    public async Task<IEnumerable<Permission>> GetPermissionsByPositionAsync(int positionId)
    {
        string key = GetPositionCacheKey(positionId);
        if (!_cache.TryGetValue(key, out IEnumerable<Permission>? entities))
        {
            entities = await _innerRepository.GetPermissionsByPositionAsync(positionId);
            _cache.Set(key, entities, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return entities ?? new List<Permission>();
    }

    public async Task<PositionPermission?> GetByPositionAndPermissionAsync(int positionId, int permissionId)
    {
        string key = GetPositionPermissionCacheKey(positionId, permissionId);
        if (!_cache.TryGetValue(key, out PositionPermission? entity))
        {
            entity = await _innerRepository.GetByPositionAndPermissionAsync(positionId, permissionId);
            if (entity != null)
            {
                _cache.Set(key, entity, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
            }
        }
        return entity;
    }

    public async Task<PositionPermission> CreateAsync(PositionPermission entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var createdEntity = await _innerRepository.CreateAsync(entity);
        InvalidateCache(entity.PositionId);
        _cache.Remove(GetPositionPermissionCacheKey(entity.PositionId, entity.PermissionId));
        return createdEntity;
    }

    public async Task<bool> DeleteAsync(int positionId, int permissionId)
    {
        var result = await _innerRepository.DeleteAsync(positionId, permissionId);
        if (result)
        {
            InvalidateCache(positionId);
            _cache.Remove(GetPositionPermissionCacheKey(positionId, permissionId));
        }
        return result;
    }

    private void InvalidateCache(int positionId)
    {
        _cache.Remove(GetPositionCacheKey(positionId));
    }

    private string GetPositionCacheKey(int positionId) => $"{_cacheKeyPrefix}_GetByPosition_{positionId}";
    private string GetPositionPermissionCacheKey(int positionId, int permissionId) => $"{_cacheKeyPrefix}_GetByPosition_{positionId}_Permission_{permissionId}";
}
