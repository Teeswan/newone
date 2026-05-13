using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedEmployeeRepository : CachedBaseRepository<Employee, int>, IEmployeeRepository
{
    private readonly IEmployeeRepository _innerRepository;

    public CachedEmployeeRepository(IEmployeeRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
        : base(innerRepository, cache, cacheDuration)
    {
        _innerRepository = innerRepository;
    }

    public override async Task<Employee> CreateAsync(Employee entity)
    {
        var result = await base.CreateAsync(entity);
        InvalidateEmployeeCustomCache(result);
        return result;
    }

    public override async Task<Employee?> UpdateAsync(Employee entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await _innerRepository.GetByIdAsync(entity.EmployeeId);
        var result = await base.UpdateAsync(entity);
        if (result != null)
        {
            InvalidateEmployeeCustomCache(result);
            if (existing != null)
            {
                if (existing.DepartmentId != result.DepartmentId && existing.DepartmentId.HasValue)
                {
                    _cache.Remove(GetDepartmentCacheKey(existing.DepartmentId.Value));
                }
                if (existing.ReportsTo != result.ReportsTo && existing.ReportsTo.HasValue)
                {
                    _cache.Remove(GetDirectReportsCacheKey(existing.ReportsTo.Value));
                }
                if (!string.Equals(existing.EmployeeCode, result.EmployeeCode, StringComparison.OrdinalIgnoreCase))
                {
                    _cache.Remove(GetCodeCacheKey(existing.EmployeeCode));
                }
            }
        }
        return result;
    }

    public override async Task<bool> DeleteAsync(int id)
    {
        var existing = await _innerRepository.GetByIdAsync(id);
        var success = await base.DeleteAsync(id);
        if (success && existing != null)
        {
            InvalidateEmployeeCustomCache(existing);
        }
        return success;
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
    {
        string key = GetDepartmentCacheKey(departmentId);
        if (!_cache.TryGetValue(key, out IEnumerable<Employee>? entities))
        {
            entities = await _innerRepository.GetEmployeesByDepartmentAsync(departmentId);
            _cache.Set(key, entities, _cacheDuration);
        }
        return entities ?? new List<Employee>();
    }

    public async Task<IEnumerable<Employee>> GetDirectReportsAsync(int managerId)
    {
        string key = GetDirectReportsCacheKey(managerId);
        if (!_cache.TryGetValue(key, out IEnumerable<Employee>? entities))
        {
            entities = await _innerRepository.GetDirectReportsAsync(managerId);
            _cache.Set(key, entities, _cacheDuration);
        }
        return entities ?? new List<Employee>();
    }

    public async Task<Employee?> GetByCodeAsync(string employeeCode)
    {
        string key = GetCodeCacheKey(employeeCode);
        if (!_cache.TryGetValue(key, out Employee? entity))
        {
            entity = await _innerRepository.GetByCodeAsync(employeeCode);
            if (entity != null)
                _cache.Set(key, entity, _cacheDuration);
        }
        return entity;
    }

    public async Task<Employee?> GetByUsernameAsync(string username)
    {
        string key = GetUsernameCacheKey(username);
        if (!_cache.TryGetValue(key, out Employee? entity))
        {
            entity = await _innerRepository.GetByUsernameAsync(username);
            if (entity != null)
                _cache.Set(key, entity, _cacheDuration);
        }
        return entity;
    }

    private void InvalidateEmployeeCustomCache(Employee employee)
    {
        if (employee.DepartmentId.HasValue)
        {
            _cache.Remove(GetDepartmentCacheKey(employee.DepartmentId.Value));
        }

        if (employee.ReportsTo.HasValue)
        {
            _cache.Remove(GetDirectReportsCacheKey(employee.ReportsTo.Value));
        }

        if (!string.IsNullOrWhiteSpace(employee.EmployeeCode))
        {
            _cache.Remove(GetCodeCacheKey(employee.EmployeeCode));
        }

        if (!string.IsNullOrWhiteSpace(employee.Username))
        {
            _cache.Remove(GetUsernameCacheKey(employee.Username));
        }
    }

    private string GetDepartmentCacheKey(int departmentId) => $"{_cacheKeyPrefix}_Dept_{departmentId}";
    private string GetDirectReportsCacheKey(int managerId) => $"{_cacheKeyPrefix}_Reports_{managerId}";
    private string GetCodeCacheKey(string employeeCode) => $"{_cacheKeyPrefix}_Code_{employeeCode}";
    private string GetUsernameCacheKey(string username) => $"{_cacheKeyPrefix}_Username_{username}";
}

public class CachedLevelRepository : CachedBaseRepository<Level, string>, ILevelRepository
{
    public CachedLevelRepository(ILevelRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
        : base(innerRepository, cache, cacheDuration) { }
}

public class CachedPositionRepository : CachedBaseRepository<Position, int>, IPositionRepository
{
    private readonly IPositionRepository _innerRepository;

    public CachedPositionRepository(IPositionRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
        : base(innerRepository, cache, cacheDuration)
    {
        _innerRepository = innerRepository;
    }

    public override async Task<Position> CreateAsync(Position entity)
    {
        var result = await base.CreateAsync(entity);
        InvalidatePositionLevelCache(result);
        return result;
    }

    public override async Task<Position?> UpdateAsync(Position entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await _innerRepository.GetByIdAsync(entity.PositionId);
        var result = await base.UpdateAsync(entity);
        if (result != null)
        {
            InvalidatePositionLevelCache(result);
            if (existing != null && !string.Equals(existing.LevelId, result.LevelId, StringComparison.OrdinalIgnoreCase))
            {
                InvalidatePositionLevelCache(existing);
            }
        }
        return result;
    }

    public override async Task<bool> DeleteAsync(int id)
    {
        var existing = await _innerRepository.GetByIdAsync(id);
        var success = await base.DeleteAsync(id);
        if (success && existing != null)
        {
            InvalidatePositionLevelCache(existing);
        }
        return success;
    }

    public async Task<IEnumerable<Position>> GetPositionsByLevelAsync(string levelId)
    {
        string key = GetLevelCacheKey(levelId);
        if (!_cache.TryGetValue(key, out IEnumerable<Position>? entities))
        {
            entities = await _innerRepository.GetPositionsByLevelAsync(levelId);
            _cache.Set(key, entities, _cacheDuration);
        }
        return entities ?? new List<Position>();
    }

    private void InvalidatePositionLevelCache(Position position)
    {
        if (!string.IsNullOrWhiteSpace(position.LevelId))
        {
            _cache.Remove(GetLevelCacheKey(position.LevelId));
        }
    }

    private string GetLevelCacheKey(string levelId) => $"{_cacheKeyPrefix}_Level_{levelId}";
}
