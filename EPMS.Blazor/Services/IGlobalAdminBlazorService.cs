using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services;

public interface IGlobalAdminBlazorService
{
    Task<GlobalAdminOrganizationDto?> GetOrganizationAsync();
    Task<List<DepartmentDto>> GetDepartmentsAsync();
    Task<List<TeamDto>> GetTeamsAsync();
    Task<List<EmployeeDto>> GetEmployeesAsync();
    Task<bool> CreateDepartmentAsync(CreateDepartmentRequest request);
    Task<bool> UpdateDepartmentAsync(int id, UpdateDepartmentRequest request);
    Task<bool> DeleteDepartmentAsync(int id);
}
