using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IPerformanceOutcomeRepository : IBaseRepository<PerformanceOutcome, int>
{
    Task<IEnumerable<PerformanceOutcome>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<PerformanceOutcome>> GetByCycleIdAsync(int cycleId);
}
