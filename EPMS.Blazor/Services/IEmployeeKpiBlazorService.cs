using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPMS.Blazor.Services
{
    public interface IEmployeeKpiBlazorService
    {
        Task<IEnumerable<EmployeeKpiDto>> GetAllAsync();
        Task<EmployeeKpiDto?> GetByIdAsync(int id);
        Task<IEnumerable<EmployeeKpiDto>> GetByEmployeeIdAsync(int employeeId);
        Task<EmployeeKpiDto> CreateAsync(EmployeeKpiRequest request);
        Task<IEnumerable<EmployeeKpiDto>> CreateBulkAsync(BulkEmployeeKpiRequest request);
        Task<EmployeeKpiDto?> UpdateAsync(int id, EmployeeKpiRequest request);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<EmployeeKpiDto>> CalculateForEmployeeAsync(int employeeId);
    }
}
