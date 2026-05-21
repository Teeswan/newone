using System.Net.Http.Json;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services;

public class AuthBlazorService : IAuthBlazorService
{
    private readonly HttpClient _httpClient;

    public AuthBlazorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        }
        return null;
    }

    public async Task<bool> ChangePasswordAsync(int employeeId, string newPassword)
    {
        var command = new { EmployeeId = employeeId, NewPassword = newPassword };
        var response = await _httpClient.PostAsJsonAsync("/api/auth/change-password", command);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateSystemSettingsAsync(string newDefaultPassword)
    {
        var command = new { NewDefaultPassword = newDefaultPassword };
        var response = await _httpClient.PostAsJsonAsync("/api/auth/update-system-settings", command);
        return response.IsSuccessStatusCode;
    }

    public async Task<BulkCreateAccountsResponse?> BulkCreateAccountsAsync()
    {
        var response = await _httpClient.PostAsync("/api/auth/bulk-create-accounts", null);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<BulkCreateAccountsResponse>();
        }
        return null;
    }
}
