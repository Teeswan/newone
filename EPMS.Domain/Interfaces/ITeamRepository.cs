using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface ITeamRepository : IBaseRepository<Team, int>
{
    Task<IEnumerable<dynamic>> GetTeamsByDepartmentAsync(int departmentId);
    Task<Team?> GetByIdNoTrackingAsync(int id);

    /// <summary>
    /// Teams belonging to a department (Department → Teams). Used for department-scoped team management.
    /// </summary>
    Task<IReadOnlyList<Team>> GetDepartmentTeamsAsync(int departmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Single team when it belongs to the given department.
    /// </summary>
    Task<Team?> GetByIdInDepartmentAsync(int teamId, int departmentId, CancellationToken cancellationToken = default);
}
