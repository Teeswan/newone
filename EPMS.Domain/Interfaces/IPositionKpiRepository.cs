using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IPositionKpiRepository
{
    Task<PositionKpi?> GetByIdAsync(int id);
    Task<IEnumerable<PositionKpi>> GetListByPositionAsync(int? positionId, bool isActive = true);
    Task<IEnumerable<PositionKpi>> GetGlobalKpisAsync(bool isActive = true);
    Task AddAsync(PositionKpi kpi);
    Task UpdateAsync(PositionKpi kpi);
    Task<bool> IsKpiReferencedByActiveCycleAsync(int kpiId);
    Task<bool> ExistsDuplicateAsync(string name, string? category, int? positionId);
}
