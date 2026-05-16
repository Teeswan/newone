using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

public interface ITeamService
{
    Task<IEnumerable<TeamDto>> GetAllAsync();
    Task<TeamDto?> GetByIdAsync(int id);
    Task<TeamDto> CreateAsync(CreateTeamRequest request);
    Task<TeamDto?> UpdateAsync(int id, UpdateTeamRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<TeamDetailDto>> GetByDepartmentAsync(int departmentId);
}
