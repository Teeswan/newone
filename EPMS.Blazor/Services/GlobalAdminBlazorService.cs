using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services;

public class GlobalAdminBlazorService : IGlobalAdminBlazorService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "api/global-admin";

    public GlobalAdminBlazorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GlobalAdminOrganizationDto?> GetOrganizationAsync()
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/organization");
        await EnsureSuccessOrThrowAsync(response);
        return await response.Content.ReadFromJsonAsync<GlobalAdminOrganizationDto>();
    }

    public async Task<List<DepartmentDto>> GetDepartmentsAsync()
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/departments");
        await EnsureSuccessOrThrowAsync(response);
        return await response.Content.ReadFromJsonAsync<List<DepartmentDto>>() ?? new List<DepartmentDto>();
    }

    public async Task<List<TeamDto>> GetTeamsAsync()
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/teams");
        await EnsureSuccessOrThrowAsync(response);
        return await response.Content.ReadFromJsonAsync<List<TeamDto>>() ?? new List<TeamDto>();
    }

    public async Task<List<EmployeeDto>> GetEmployeesAsync()
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/employees");
        await EnsureSuccessOrThrowAsync(response);
        return await response.Content.ReadFromJsonAsync<List<EmployeeDto>>() ?? new List<EmployeeDto>();
    }

    public async Task<bool> CreateDepartmentAsync(CreateDepartmentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/departments", request);
        await EnsureSuccessOrThrowAsync(response);
        return true;
    }

    public async Task<bool> UpdateDepartmentAsync(int id, UpdateDepartmentRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/departments/{id}", request);
        await EnsureSuccessOrThrowAsync(response);
        return true;
    }

    public async Task<bool> DeleteDepartmentAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/departments/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        await EnsureSuccessOrThrowAsync(response);
        return true;
    }

    private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            throw new UnauthorizedAccessException(string.IsNullOrWhiteSpace(errorContent) ? "Access Denied" : errorContent);
        }

        throw new HttpRequestException(errorContent ?? response.ReasonPhrase, null, response.StatusCode);
    }
}
