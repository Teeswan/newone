using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using EPMS.Shared.Common;
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
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<PositionDto>>>();
            return result?.Data ?? new List<PositionDto>();
        }
        return new List<PositionDto>();
    }

    public async Task<PositionDto?> GetPositionByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<PositionDto>>();
            return result?.Data;
        }
        return null;
    }

    public async Task<PositionDto> CreatePositionAsync(CreatePositionRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PositionDto>>();
        
        if (response.IsSuccessStatusCode && result != null && result.Success)
        {
            return result.Data!;
        }
        
        throw new InvalidOperationException(result?.Message ?? "Could not create position.");
    }

    public async Task<PositionDto?> UpdatePositionAsync(int id, UpdatePositionRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PositionDto>>();
        
        if (response.IsSuccessStatusCode && result != null && result.Success)
        {
            return result.Data;
        }
        
        throw new InvalidOperationException(result?.Message ?? "Could not update position.");
    }

    public async Task<bool> DeletePositionAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<PositionDto>> GetPositionsByLevelAsync(string levelId)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/level/{levelId}");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<PositionDto>>>();
            return result?.Data ?? new List<PositionDto>();
        }
        return new List<PositionDto>();
    }
}
