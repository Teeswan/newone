using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface ITeamRepository : IBaseRepository<Team, int>
{
    Task<IEnumerable<dynamic>> GetTeamsByDepartmentAsync(int departmentId);
}
