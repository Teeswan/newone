using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IKpiAssignmentRepository
{
    Task<IEnumerable<EmployeeKpiAssignment>> GetAssignmentsByEmployeeCycleAsync(int employeeId, int cycleId);
    Task<EmployeeKpiAssignment?> GetByIdAsync(int assignmentId);
    Task<EmployeeKpiAssignment?> GetSnapshotAsync(int employeeId, int cycleId, int kpiId);
    Task AddRangeAsync(IEnumerable<EmployeeKpiAssignment> assignments);
    Task AddAsync(EmployeeKpiAssignment assignment);
    Task UpdateAsync(EmployeeKpiAssignment assignment);
}
