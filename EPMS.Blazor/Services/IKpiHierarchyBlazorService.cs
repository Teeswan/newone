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

        // Team KPIs
        Task<IEnumerable<TeamKpiDto>> GetAllTeamAsync();
        Task<IEnumerable<TeamKpiDto>> GetTeamByTeamAsync(int teamId);
        Task<TeamKpiDto> CreateTeamAsync(TeamKpiRequest request);

        // Employee KPIs
        Task<IEnumerable<EmployeeKpiDto>> GetAllEmpAsync();
        Task<IEnumerable<EmployeeKpiDto>> GetEmpByEmpAsync(int empId);
        Task<EmployeeKpiDto> CreateEmpAsync(EmployeeKpiRequest request);
        Task<EmployeeKpiDto?> UpdateEmpAsync(int id, EmployeeKpiRequest request);
    }
}
