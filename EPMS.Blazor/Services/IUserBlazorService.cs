using EPMS.Shared.DTOs;

namespace EPMS.Blazor.Services
{
    public interface IUserBlazorService
    {
        Task<List<UserRoleDto>> GetAllUsersAsync();
    }
}
