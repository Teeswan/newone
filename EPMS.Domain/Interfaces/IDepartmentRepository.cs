using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IDepartmentRepository : IBaseRepository<Department, int>
{
    Task<IEnumerable<Department>> GetDepartmentTreeAsync();
}
