using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IEmployeeRepository : IBaseRepository<Employee, int>
{
    Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
    Task<IEnumerable<Employee>> GetEmployeesByTeamAsync(int teamId);
    Task<IEnumerable<Employee>> GetDirectReportsAsync(int managerId);
    Task<Employee?> GetByCodeAsync(string employeeCode);
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee?> GetByIdNoTrackingAsync(int id);

    /// <summary>
    /// Loads an employee with their <see cref="Employee.TeamsNavigation"/> collection (M:N via TeamMembers).
    /// </summary>
    Task<Employee?> GetWithTeamsAsync(int employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns team IDs the employee belongs to (from TeamMembers junction).
    /// </summary>
    Task<IReadOnlyList<int>> GetTeamIdsForEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns employees who share at least one team with the given employee (many-to-many filter).
    /// </summary>
    Task<IReadOnlyList<Employee>> GetEmployeesSharingTeamsWithAsync(int currentEmployeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// True when <paramref name="targetEmployeeId"/> shares at least one team with <paramref name="currentEmployeeId"/>.
    /// </summary>
    Task<bool> SharesAnyTeamWithAsync(int currentEmployeeId, int targetEmployeeId, CancellationToken cancellationToken = default);
}
