using EPMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPMS.Domain.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<IEnumerable<UserRole>> GetAllUserRolesAsync();
    }
}