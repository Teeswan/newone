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
        Console.WriteLine($"AuthBlazorService.ChangePasswordAsync called - employeeId: {employeeId}, newPassword: {newPassword}");
        var request = new ChangePasswordRequest
        {
            EmployeeId = employeeId,
            NewPassword = newPassword
        };
        var response = await _httpClient.PostAsJsonAsync("/api/auth/change-password", request);
        Console.WriteLine($"AuthBlazorService.ChangePasswordAsync response status: {response.StatusCode}");
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"AuthBlazorService.ChangePasswordAsync error content: {errorContent}");
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateSystemSettingsAsync(string newDefaultPassword)
    {
        var request = new UpdateSystemSettingsRequest
        {
            NewDefaultPassword = newDefaultPassword
        };
        var response = await _httpClient.PostAsJsonAsync("/api/auth/update-system-settings", request);
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
