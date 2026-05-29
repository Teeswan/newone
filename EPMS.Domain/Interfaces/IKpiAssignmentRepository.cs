using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IKpiAssignmentRepository : IBaseRepository<EmployeeKpi, int>
{
    Task<IEnumerable<EmployeeKpi>> GetAssignmentsByEmployeeCycleAsync(int employeeId, int cycleId);
    Task<EmployeeKpi?> GetSnapshotAsync(int employeeId, int cycleId, int kpiId);
    Task AddRangeAsync(IEnumerable<EmployeeKpi> assignments);
    Task AddAsync(EmployeeKpi assignment);
}
