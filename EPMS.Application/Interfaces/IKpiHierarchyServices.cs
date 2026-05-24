using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPMS.Application.Interfaces
{
    public interface IKpiService
    {
        Task<IEnumerable<KpiDto>> GetAllAsync();
        Task<KpiDto?> GetByIdAsync(int id);
        Task<KpiDto> CreateAsync(KpiRequest request, int? createdByEmployeeId);
        Task<KpiDto?> UpdateAsync(int id, KpiRequest request);
        Task<bool> DeleteAsync(int id);
    }

    public interface IDepartmentKpiService
    {
        Task<IEnumerable<DepartmentKpiDto>> GetAllAsync();
        Task<DepartmentKpiDto?> GetByIdAsync(int id);
        Task<IEnumerable<DepartmentKpiDto>> GetByDepartmentIdAsync(int departmentId, int cycleId);
        Task<DepartmentKpiDto> CreateAsync(DepartmentKpiRequest request);
        Task<DepartmentKpiDto?> UpdateAsync(int id, DepartmentKpiRequest request);
        Task<bool> DeleteAsync(int id);
    }

    public interface ITeamKpiService
    {
        Task<IEnumerable<TeamKpiDto>> GetAllAsync();
        Task<TeamKpiDto?> GetByIdAsync(int id);
        Task<IEnumerable<TeamKpiDto>> GetByTeamIdAsync(int teamId);
        Task<TeamKpiDto> CreateAsync(TeamKpiRequest request);
        Task<TeamKpiDto?> UpdateAsync(int id, TeamKpiRequest request);
        Task<bool> DeleteAsync(int id);
    }

    public interface IEmployeeKpiService
    {
        Task<IEnumerable<EmployeeKpiDto>> GetAllAsync();
        Task<EmployeeKpiDto?> GetByIdAsync(int id);
        Task<IEnumerable<EmployeeKpiDto>> GetByEmployeeIdAsync(int employeeId);
        Task<EmployeeKpiDto> CreateAsync(EmployeeKpiRequest request);
        Task<EmployeeKpiDto?> UpdateAsync(int id, EmployeeKpiRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
