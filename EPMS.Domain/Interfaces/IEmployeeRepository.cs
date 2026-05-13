using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IEmployeeRepository : IBaseRepository<Employee, int>
{
    Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
    Task<IEnumerable<Employee>> GetDirectReportsAsync(int managerId);
    Task<Employee?> GetByCodeAsync(string employeeCode);
    Task<Employee?> GetByUsernameAsync(string username);
}
