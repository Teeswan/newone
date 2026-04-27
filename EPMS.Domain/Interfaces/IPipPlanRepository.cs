using EPMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Domain.Interfaces
{
    public interface IPipPlanRepository
    {
        Task<PipPlan?> GetByIdAsync(int pipId, CancellationToken ct = default);
        Task<PipPlan?> GetByIdWithDetailsAsync(int pipId, CancellationToken ct = default);
        Task<IEnumerable<PipPlan>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<PipPlan>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default);
        Task<IEnumerable<PipPlan>> GetByManagerIdAsync(int managerId, CancellationToken ct = default);
        Task<bool> HasActivePipAsync(int employeeId, CancellationToken ct = default);
        Task<PipPlan> CreateAsync(PipPlan plan, CancellationToken ct = default);
        Task<PipPlan> UpdateAsync(PipPlan plan, CancellationToken ct = default);
        Task<bool> DeleteAsync(int pipId, CancellationToken ct = default);
    }
}
