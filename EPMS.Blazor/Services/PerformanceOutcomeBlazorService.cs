using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services
{
    public class PerformanceOutcomeBlazorService : IPerformanceOutcomeBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/PerformanceOutcomes";

        public PerformanceOutcomeBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<PerformanceOutcomeDto>> GetAllPerformanceOutcomesAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<PerformanceOutcomeDto>>() ?? new List<PerformanceOutcomeDto>();
        }

        public async Task<PerformanceOutcomeDto?> GetPerformanceOutcomeByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PerformanceOutcomeDto>();
        }

        public async Task<PerformanceOutcomeDto> CreatePerformanceOutcomeAsync(CreatePerformanceOutcomeRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PerformanceOutcomeDto>() ?? throw new InvalidOperationException("Could not create performance outcome.");
        }

        public async Task<PerformanceOutcomeDto?> UpdatePerformanceOutcomeAsync(int id, UpdatePerformanceOutcomeRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PerformanceOutcomeDto>();
        }

        public async Task<bool> DeletePerformanceOutcomeAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
