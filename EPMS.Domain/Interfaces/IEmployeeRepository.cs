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
}
