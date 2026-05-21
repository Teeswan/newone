using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface ISystemSettingRepository
{
    Task<string?> GetValueAsync(string key);
    Task SetValueAsync(string key, string value);
}
