using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services
{
    public interface IEmployeeBlazorService
    {
      
        Task<List<EmployeeDto>> GetEmployeesAsync();
        Task<EmployeeDetailDto?> GetEmployeeDetailsAsync(int id);
        Task<List<EmployeeDto>> GetHierarchyAsync();
        Task<bool> CreateEmployeeAsync(CreateEmployeeRequest request);
        Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
