using System;
using System.Threading.Tasks;

namespace EPMS.Application.Interfaces;

public interface IKpiCacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
}
