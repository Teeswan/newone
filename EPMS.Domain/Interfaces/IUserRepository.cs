using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IUserRepository : IBaseRepository<User, int>
{
    Task<User?> GetByEmployeeIdAsync(int employeeId);
}
