using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedBaseRepository<T, TKey> : IBaseRepository<T, TKey> where T : class
{
    private readonly IBaseRepository<T, TKey> _innerRepository;
    protected readonly IMemoryCache _cache;
    protected readonly string _cacheKeyPrefix;
    protected readonly TimeSpan _cacheDuration;
    private static PropertyInfo? _idProperty;

    public CachedBaseRepository(IBaseRepository<T, TKey> innerRepository, IMemoryCache cache, TimeSpan? cacheDuration = null)
    {
        _innerRepository = innerRepository ?? throw new ArgumentNullException(nameof(innerRepository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheKeyPrefix = typeof(T).Name;
        _cacheDuration = cacheDuration ?? TimeSpan.FromMinutes(10);

        // One-time reflection to find the ID property
        if (_idProperty == null)
        {
            // Try to find property with "Id" or ending with "Id" (like CycleId, FormId)
            _idProperty = typeof(T).GetProperties()
                .FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) || 
                                     p.Name.Equals($"{typeof(T).Name}Id", StringComparison.OrdinalIgnoreCase) ||
                                     p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        string key = $"{_cacheKeyPrefix}_GetAll";
        if (!_cache.TryGetValue(key, out IEnumerable<T>? entities))
        {
            entities = await _innerRepository.GetAllAsync();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(_cacheDuration);
            _cache.Set(key, entities, cacheOptions);
        }
        return entities ?? new List<T>();
    }

    public virtual async Task<T?> GetByIdAsync(TKey id)
    {
        string key = $"{_cacheKeyPrefix}_GetById_{id}";
        if (!_cache.TryGetValue(key, out T? entity))
        {
            entity = await _innerRepository.GetByIdAsync(id);
            if (entity != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_cacheDuration);
                _cache.Set(key, entity, cacheOptions);
            }
        }
        return entity;
    }

    public virtual async Task<T?> GetByIdFromDbAsync(TKey id)
    {
        // For cached repository, "FromDb" means bypassing the cache and going to the inner repository
        return await _innerRepository.GetByIdFromDbAsync(id);
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var createdEntity = await _innerRepository.CreateAsync(entity);
        InvalidateCache();
        return createdEntity;
    }

    public virtual async Task<T?> UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var updatedEntity = await _innerRepository.UpdateAsync(entity);
        InvalidateCache();
        
        // Generic ID invalidation
        if (updatedEntity != null && _idProperty != null)
        {
            var idValue = _idProperty.GetValue(updatedEntity);
            if (idValue is TKey id)
            {
                InvalidateItemCache(id);
            }
        }
        
        return updatedEntity;
    }

    public virtual async Task<bool> DeleteAsync(TKey id)
    {
        var result = await _innerRepository.DeleteAsync(id);
        if (result)
        {
            InvalidateCache();
            InvalidateItemCache(id);
        }
        return result;
    }

    protected virtual void InvalidateCache()
    {
        _cache.Remove($"{_cacheKeyPrefix}_GetAll");
    }

    protected virtual void InvalidateItemCache(TKey id)
    {
        _cache.Remove($"{_cacheKeyPrefix}_GetById_{id}");
    }
}
