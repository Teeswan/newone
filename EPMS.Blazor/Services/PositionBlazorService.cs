using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services;

public class PositionBlazorService : IPositionBlazorService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "api/Positions";

    public PositionBlazorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<PositionDto>> GetAllPositionsAsync()
    {
        var response = await _httpClient.GetAsync(BaseUrl);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<PositionDto>>() ?? new List<PositionDto>();
    }

    public async Task<PositionDto?> GetPositionByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PositionDto>();
    }

    public async Task<PositionDto> CreatePositionAsync(CreatePositionRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PositionDto>() ?? throw new InvalidOperationException("Could not create position.");
    }

    public async Task<PositionDto?> UpdatePositionAsync(int id, UpdatePositionRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PositionDto>();
    }

    public async Task<bool> DeletePositionAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<PositionDto>> GetPositionsByLevelAsync(string levelId)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/level/{levelId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<PositionDto>>() ?? new List<PositionDto>();
    }
}
