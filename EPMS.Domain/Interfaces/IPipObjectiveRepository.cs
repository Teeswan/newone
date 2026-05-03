using EPMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Domain.Interfaces
{
    public interface IPipObjectiveRepository
    {
        Task<PipObjective?> GetByIdAsync(int objectiveId, CancellationToken ct = default);
        Task<IEnumerable<PipObjective>> GetByPipIdAsync(int pipId, CancellationToken ct = default);
        Task<PipObjective> CreateAsync(PipObjective objective, CancellationToken ct = default);
        Task<PipObjective> UpdateAsync(PipObjective objective, CancellationToken ct = default);
        Task<bool> DeleteAsync(int objectiveId, CancellationToken ct = default);
        Task<int> GetAchievedCountByPipIdAsync(int pipId, CancellationToken ct = default);
    }
}
