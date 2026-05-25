using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using EPMS.Shared.Common;

namespace EPMS.Blazor.Services;

public interface IAuthBlazorService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequest request);
    Task<bool> ChangePasswordAsync(int employeeId, string newPassword);
    Task<bool> UpdateSystemSettingsAsync(string newDefaultPassword);
    Task<BulkCreateAccountsResponse?> BulkCreateAccountsAsync();
    Task<Result?> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<Result?> ResetPasswordAsync(ResetPasswordRequest request);
}
