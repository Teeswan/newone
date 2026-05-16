using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedDepartmentRepository : CachedBaseRepository<Department, int>, IDepartmentRepository
{
    private readonly IDepartmentRepository _innerRepository;

    public CachedDepartmentRepository(IDepartmentRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration) 
        : base(innerRepository, cache, cacheDuration)
    {
        _innerRepository = innerRepository;
    }

    public async Task<IEnumerable<Department>> GetDepartmentTreeAsync()
    {
        string key = $"{_cacheKeyPrefix}_GetTree";
        if (!_cache.TryGetValue(key, out IEnumerable<Department>? entities))
        {
            entities = await _innerRepository.GetDepartmentTreeAsync();
            _cache.Set(key, entities, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return entities ?? new List<Department>();
    }

    protected override void InvalidateCache()
    {
        base.InvalidateCache();
        _cache.Remove($"{_cacheKeyPrefix}_GetTree");
    }
}
