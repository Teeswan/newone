using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services;

public interface IAuthBlazorService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequest request);
    Task<bool> ChangePasswordAsync(int employeeId, string newPassword);
    Task<bool> UpdateSystemSettingsAsync(string newDefaultPassword);
    Task<BulkCreateAccountsResponse?> BulkCreateAccountsAsync();
}
