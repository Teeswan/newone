using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services
{
    public interface ITeamBlazorService
    {
        Task<IEnumerable<TeamDto>> GetAllTeamsAsync();
        Task<TeamDto?> GetTeamByIdAsync(int id);
        Task<TeamDto> CreateTeamAsync(CreateTeamRequest request);
        Task<TeamDto?> UpdateTeamAsync(int id, UpdateTeamRequest request);
        Task<bool> DeleteTeamAsync(int id);
        Task<IEnumerable<TeamDetailDto>> GetTeamsByDepartmentAsync(int departmentId);
        Task<byte[]> ExportToExcelAsync();
        Task<byte[]> ExportToPdfAsync();
        Task<int> ImportFromExcelAsync(Stream fileStream, bool skipFirstRow = true, string sheetName = "", bool skipExisting = false);

        Task<List<TeamDto>> GetDepartmentScopedManageableTeamsAsync();
        Task<TeamDto?> GetDepartmentScopedManageableTeamAsync(int id);
        Task<bool> UpdateDepartmentScopedManageableTeamAsync(int id, UpdateTeamRequest request);
        Task<DepartmentScopedAccessDto?> GetDepartmentScopedAccessAsync();
    }
}
