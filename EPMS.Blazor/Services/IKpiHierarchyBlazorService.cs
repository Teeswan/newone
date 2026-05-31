using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPMS.Blazor.Services
{
    public interface IKpiHierarchyBlazorService
    {
        // Master KPIs
        Task<IEnumerable<KpiDto>> GetAllMasterAsync();
        Task<KpiDto?> GetMasterByIdAsync(int id);
        Task<KpiDto> CreateMasterAsync(KpiRequest request);
        Task<KpiDto?> UpdateMasterAsync(int id, KpiRequest request);
        Task<bool> DeleteMasterAsync(int id);

        // Department KPIs
        Task<IEnumerable<DepartmentKpiDto>> GetAllDeptAsync();
        Task<IEnumerable<DepartmentKpiDto>> GetDeptByDeptAsync(int deptId, int cycleId);
        Task<DepartmentKpiDto> CreateDeptAsync(DepartmentKpiRequest request);
        Task<DepartmentKpiDto?> UpdateDeptAsync(int id, DepartmentKpiRequest request);
        Task<IEnumerable<DepartmentKpiDto>> CalculateDepartmentAsync(int deptId);

        // Team KPIs
        Task<IEnumerable<TeamKpiDto>> GetAllTeamAsync();
        Task<IEnumerable<TeamKpiDto>> GetTeamByTeamAsync(int teamId);
        Task<TeamKpiDto> CreateTeamAsync(TeamKpiRequest request);
        Task<TeamKpiDto?> UpdateTeamAsync(int id, TeamKpiRequest request);
        Task<IEnumerable<TeamKpiDto>> CalculateTeamAsync(int teamId);
    }
}
