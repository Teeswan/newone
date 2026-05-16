using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services;

public class PermissionBlazorService : IPermissionBlazorService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "api/Permissions";

    public PermissionBlazorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
    {
        var response = await _httpClient.GetAsync(BaseUrl);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<PermissionDto>>() ?? new List<PermissionDto>();
    }

    public async Task<IEnumerable<PermissionDto>> GetPermissionsByPositionAsync(int positionId)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/position/{positionId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<PermissionDto>>() ?? new List<PermissionDto>();
    }

    public async Task<bool> AssignPermissionAsync(AssignPermissionRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/assign", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RevokePermissionAsync(RevokePermissionRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/revoke", request);
        return response.IsSuccessStatusCode;
    }
}
