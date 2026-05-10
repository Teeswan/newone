using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services
{
    public interface IDepartmentBlazorService
    {
        Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
        Task<DepartmentDto?> GetDepartmentByIdAsync(int id);
        Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentRequest request);
        Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentRequest request);
        Task<bool> DeleteDepartmentAsync(int id);
        Task<IEnumerable<DepartmentTreeDto>> GetDepartmentTreeAsync();
        Task<byte[]> ExportToExcelAsync();
        Task<byte[]> ExportToPdfAsync();
        Task<int> ImportFromExcelAsync(Stream fileStream, bool skipFirstRow = true, string sheetName = "", bool skipExisting = false);
    }
}
