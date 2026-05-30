using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using EPMS.Shared.Common;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services;

public class LevelBlazorService : ILevelBlazorService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "api/Levels";

    public LevelBlazorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<LevelDto>> GetAllLevelsAsync()
    {
        var response = await _httpClient.GetAsync(BaseUrl);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<LevelDto>>>();
            return result?.Data ?? new List<LevelDto>();
        }
        
        // Handle direct array response for backward compatibility or different API design
        try { return await response.Content.ReadFromJsonAsync<IEnumerable<LevelDto>>() ?? new List<LevelDto>(); }
        catch { return new List<LevelDto>(); }
    }

    public async Task<LevelDto?> GetLevelByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<LevelDto>>();
            return result?.Data;
        }
        return null;
    }

    public async Task<LevelDto> CreateLevelAsync(CreateLevelRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<LevelDto>>();
        
        if (response.IsSuccessStatusCode && result != null && result.Success)
        {
            return result.Data!;
        }
        
        throw new InvalidOperationException(result?.Message ?? "Could not create level.");
    }

    public async Task<LevelDto?> UpdateLevelAsync(string id, UpdateLevelRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<LevelDto>>();
        
        if (response.IsSuccessStatusCode && result != null && result.Success)
        {
            return result.Data;
        }
        
        throw new InvalidOperationException(result?.Message ?? "Could not update level.");
    }

    public async Task<bool> DeleteLevelAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
        return response.IsSuccessStatusCode;
    }
}
