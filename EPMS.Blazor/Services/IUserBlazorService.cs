using EPMS.Shared.DTOs;

namespace EPMS.Blazor.Services
{
    public interface IUserBlazorService
    {
        Task<List<UserDto>> GetAllUsersAsync();
    }
}
