using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedTeamRepository : CachedBaseRepository<Team, int>, ITeamRepository
{
    private readonly ITeamRepository _innerRepository;

    public CachedTeamRepository(ITeamRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
        : base(innerRepository, cache, cacheDuration)
    {
        _innerRepository = innerRepository;
    }

    public override async Task<Team> CreateAsync(Team entity)
    {
        var result = await base.CreateAsync(entity);
        if (result.DepartmentId.HasValue)
        {
            _cache.Remove(GetDepartmentCacheKey(result.DepartmentId.Value));
        }
        return result;
    }

    public override async Task<Team?> UpdateAsync(Team entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await _innerRepository.GetByIdNoTrackingAsync(entity.TeamId);
        var result = await base.UpdateAsync(entity);
        if (result != null)
        {
            if (result.DepartmentId.HasValue)
            {
                _cache.Remove(GetDepartmentCacheKey(result.DepartmentId.Value));
            }

            if (existing != null && existing.DepartmentId != result.DepartmentId && existing.DepartmentId.HasValue)
            {
                _cache.Remove(GetDepartmentCacheKey(existing.DepartmentId.Value));
            }
        }
        return result;
    }

    public override async Task<bool> DeleteAsync(int id)
    {
        var existing = await _innerRepository.GetByIdNoTrackingAsync(id);
        var success = await base.DeleteAsync(id);
        if (success && existing?.DepartmentId.HasValue == true)
        {
            _cache.Remove(GetDepartmentCacheKey(existing.DepartmentId.Value));
        }
        return success;
    }

    public async Task<IEnumerable<dynamic>> GetTeamsByDepartmentAsync(int departmentId)
    {
        string key = GetDepartmentCacheKey(departmentId);
        if (!_cache.TryGetValue(key, out IEnumerable<dynamic>? entities))
        {
            entities = await _innerRepository.GetTeamsByDepartmentAsync(departmentId);
            _cache.Set(key, entities, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        }
        return entities ?? new List<dynamic>();
    }

    public async Task<Team?> GetByIdNoTrackingAsync(int id)
    {
        return await _innerRepository.GetByIdNoTrackingAsync(id);
    }

    public Task<IReadOnlyList<Team>> GetDepartmentTeamsAsync(int departmentId, CancellationToken cancellationToken = default)
        => _innerRepository.GetDepartmentTeamsAsync(departmentId, cancellationToken);

    public Task<Team?> GetByIdInDepartmentAsync(int teamId, int departmentId, CancellationToken cancellationToken = default)
        => _innerRepository.GetByIdInDepartmentAsync(teamId, departmentId, cancellationToken);

    private static string GetDepartmentCacheKey(int departmentId) => $"Team_GetByDepartment_{departmentId}";
}
