using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedAppraisalQuestionRepository : CachedBaseRepository<AppraisalQuestion, int>, IAppraisalQuestionRepository
{
    public CachedAppraisalQuestionRepository(IAppraisalQuestionRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration)
        : base(innerRepository, cache, cacheDuration)
    {
    }
}
