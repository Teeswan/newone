using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IPerformanceEvaluationRepository : IBaseRepository<PerformanceEvaluation, int>
{
    Task<IEnumerable<PerformanceEvaluation>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<PerformanceEvaluation>> GetByCycleIdAsync(int cycleId);
}
