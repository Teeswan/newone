using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedPerformanceOutcomeRepository : CachedBaseRepository<PerformanceOutcome, int>, IPerformanceOutcomeRepository
{
    private readonly IPerformanceOutcomeRepository _innerRepository;

    public CachedPerformanceOutcomeRepository(IPerformanceOutcomeRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
        : base(innerRepository, cache, cacheDuration)
    {
        _innerRepository = innerRepository;
    }

    public override async Task<PerformanceOutcome> CreateAsync(PerformanceOutcome entity)
    {
        var result = await base.CreateAsync(entity);
        InvalidateRelatedCaches(result);
        return result;
    }

    public override async Task<PerformanceOutcome?> UpdateAsync(PerformanceOutcome entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await _innerRepository.GetByIdAsync(entity.OutcomeId);
        var result = await base.UpdateAsync(entity);
        if (result != null)
        {
            InvalidateItemCache(result.OutcomeId);
            InvalidateRelatedCaches(result);

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
            InvalidateRelatedCaches(existing);
        }
        return success;
    }

    public async Task<IEnumerable<PerformanceOutcome>> GetByEmployeeIdAsync(int employeeId)
    {
        string key = GetEmployeeCacheKey(employeeId);
        if (!_cache.TryGetValue(key, out IEnumerable<PerformanceOutcome>? entities))
        {
            entities = await _innerRepository.GetByEmployeeIdAsync(employeeId);
            _cache.Set(key, entities, _cacheDuration);
        }
        return entities ?? new List<PerformanceOutcome>();
    }

    public async Task<IEnumerable<PerformanceOutcome>> GetByCycleIdAsync(int cycleId)
    {
        string key = GetCycleCacheKey(cycleId);
        if (!_cache.TryGetValue(key, out IEnumerable<PerformanceOutcome>? entities))
        {
            entities = await _innerRepository.GetByCycleIdAsync(cycleId);
            _cache.Set(key, entities, _cacheDuration);
        }
        return entities ?? new List<PerformanceOutcome>();
    }

    private void InvalidateRelatedCaches(PerformanceOutcome outcome)
    {
        if (outcome.EmployeeId.HasValue)
        {
            _cache.Remove(GetEmployeeCacheKey(outcome.EmployeeId.Value));
        }
        if (outcome.CycleId.HasValue)
        {
            _cache.Remove(GetCycleCacheKey(outcome.CycleId.Value));
        }
    }

    private static string GetEmployeeCacheKey(int employeeId) => $"PerformanceOutcome_GetByEmployeeId_{employeeId}";
    private static string GetCycleCacheKey(int cycleId) => $"PerformanceOutcome_GetByCycleId_{cycleId}";
}
