using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IAccountInitializationService
{
    Task InitializeAccountAsync(Employee employee, CancellationToken cancellationToken = default);
}
