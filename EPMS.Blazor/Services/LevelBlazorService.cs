using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
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
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<LevelDto>>() ?? new List<LevelDto>();
    }

    public async Task<LevelDto?> GetLevelByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LevelDto>();
    }

    public async Task<LevelDto> CreateLevelAsync(CreateLevelRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LevelDto>() ?? throw new InvalidOperationException("Could not create level.");
    }

    public async Task<LevelDto?> UpdateLevelAsync(string id, UpdateLevelRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LevelDto>();
    }

    public async Task<bool> DeleteLevelAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
        return response.IsSuccessStatusCode;
    }
}
