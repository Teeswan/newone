using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface ISystemSettingRepository
{
    Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default);
    Task SetValueAsync(string key, string value, CancellationToken cancellationToken = default);
}
