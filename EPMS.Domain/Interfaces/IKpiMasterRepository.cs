using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IKpiMasterRepository
{
    Task<KpiMaster?> GetByIdAsync(int id);
    Task<IEnumerable<KpiMaster>> GetListByPositionAsync(int? positionId, bool isActive = true);
    Task<IEnumerable<KpiMaster>> GetGlobalKpisAsync(bool isActive = true);
    Task AddAsync(KpiMaster kpi);
    Task UpdateAsync(KpiMaster kpi);
    Task<bool> IsKpiReferencedByActiveCycleAsync(int kpiId);
    Task<bool> ExistsDuplicateAsync(string name, string? category, int? positionId);
}
