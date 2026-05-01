using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories;

public class CachedAppraisalFormRepository : CachedBaseRepository<ApplicationForm, int>, IAppraisalFormRepository
{
    public CachedAppraisalFormRepository(IAppraisalFormRepository innerRepository, IMemoryCache cache, TimeSpan cacheDuration) 
        : base(innerRepository, cache, cacheDuration)
    {
    }
}
