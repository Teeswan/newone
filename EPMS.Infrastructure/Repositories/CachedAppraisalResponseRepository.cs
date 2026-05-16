using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedAppraisalResponseRepository : CachedBaseRepository<AppraisalResponse, long>, IAppraisalResponseRepository
{
    private readonly IAppraisalResponseRepository _innerRepository;

    public CachedAppraisalResponseRepository(IAppraisalResponseRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
        : base(innerRepository, cache, cacheDuration)
    {
        _innerRepository = innerRepository;
    }

    public override async Task<AppraisalResponse> CreateAsync(AppraisalResponse entity)
    {
        var result = await base.CreateAsync(entity);
        _cache.Remove($"AppraisalResponse_GetByEvalId_{result.EvalId}");
        return result;
    }

    public override async Task<AppraisalResponse?> UpdateAsync(AppraisalResponse entity)
    {
        var existing = await _innerRepository.GetByIdAsync(entity.ResponseId);
        var result = await base.UpdateAsync(entity);
        if (result != null)
        {
            InvalidateItemCache(result.ResponseId);
            _cache.Remove($"AppraisalResponse_GetByEvalId_{result.EvalId}");
            if (existing != null && existing.EvalId != result.EvalId)
            {
                _cache.Remove($"AppraisalResponse_GetByEvalId_{existing.EvalId}");
            }
        }
        return result;
    }

    public override async Task<bool> DeleteAsync(long id)
    {
        var existing = await _innerRepository.GetByIdAsync(id);
        var success = await base.DeleteAsync(id);
        if (success && existing != null)
        {
            _cache.Remove($"AppraisalResponse_GetByEvalId_{existing.EvalId}");
        }
        return success;
    }

    public async Task<IEnumerable<AppraisalResponse>> GetByEvalIdAsync(int evalId)
    {
        string key = $"AppraisalResponse_GetByEvalId_{evalId}";
        if (!_cache.TryGetValue(key, out IEnumerable<AppraisalResponse>? entities))
        {
            entities = await _innerRepository.GetByEvalIdAsync(evalId);
            _cache.Set(key, entities, _cacheDuration);
        }
        return entities ?? new List<AppraisalResponse>();
    }
}
