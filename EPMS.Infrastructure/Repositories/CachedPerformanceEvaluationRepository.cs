using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedPerformanceEvaluationRepository : CachedBaseRepository<PerformanceEvaluation, int>, IPerformanceEvaluationRepository
{
    private readonly IPerformanceEvaluationRepository _innerRepository;

    public CachedPerformanceEvaluationRepository(IPerformanceEvaluationRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
        : base(innerRepository, cache, cacheDuration)
    {
        _innerRepository = innerRepository;
    }

    public override async Task<PerformanceEvaluation> CreateAsync(PerformanceEvaluation entity)
    {
        var result = await base.CreateAsync(entity);
        InvalidateCustomCaches(result);
        return result;
    }

    public override async Task<PerformanceEvaluation?> UpdateAsync(PerformanceEvaluation entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await _innerRepository.GetByIdAsync(entity.EvalId);
        var result = await base.UpdateAsync(entity);
        if (result != null)
        {
            InvalidateItemCache(result.EvalId);
            InvalidateCustomCaches(result);

            if (existing != null)
            {
                if (existing.EmployeeId != result.EmployeeId && existing.EmployeeId.HasValue)
                {
                    _cache.Remove(GetEmployeeCacheKey(existing.EmployeeId.Value));
                }
                if (existing.CycleId != result.CycleId && existing.CycleId.HasValue)
                {
                    _cache.Remove(GetCycleCacheKey(existing.CycleId.Value));
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
            InvalidateCustomCaches(existing);
        }
        return success;
    }

    public async Task<IEnumerable<PerformanceEvaluation>> GetByEmployeeIdAsync(int employeeId)
    {
        string key = GetEmployeeCacheKey(employeeId);
        if (!_cache.TryGetValue(key, out IEnumerable<PerformanceEvaluation>? entities))
        {
            entities = await _innerRepository.GetByEmployeeIdAsync(employeeId);
            _cache.Set(key, entities, _cacheDuration);
        }
        return entities ?? new List<PerformanceEvaluation>();
    }

    public async Task<IEnumerable<PerformanceEvaluation>> GetByCycleIdAsync(int cycleId)
    {
        string key = GetCycleCacheKey(cycleId);
        if (!_cache.TryGetValue(key, out IEnumerable<PerformanceEvaluation>? entities))
        {
            entities = await _innerRepository.GetByCycleIdAsync(cycleId);
            _cache.Set(key, entities, _cacheDuration);
        }
        return entities ?? new List<PerformanceEvaluation>();
    }

    private void InvalidateCustomCaches(PerformanceEvaluation entity)
    {
        if (entity.EmployeeId.HasValue)
        {
            _cache.Remove(GetEmployeeCacheKey(entity.EmployeeId.Value));
        }
        if (entity.CycleId.HasValue)
        {
            _cache.Remove(GetCycleCacheKey(entity.CycleId.Value));
        }
    }

    private static string GetEmployeeCacheKey(int employeeId) => $"PerformanceEvaluation_GetByEmployeeId_{employeeId}";
    private static string GetCycleCacheKey(int cycleId) => $"PerformanceEvaluation_GetByCycleId_{cycleId}";
}
