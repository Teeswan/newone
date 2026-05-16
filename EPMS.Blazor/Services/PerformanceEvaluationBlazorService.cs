using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services
{
    public class PerformanceEvaluationBlazorService : IPerformanceEvaluationBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/PerformanceEvaluations";

        public PerformanceEvaluationBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<PerformanceEvaluationDto>> GetAllPerformanceEvaluationsAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<PerformanceEvaluationDto>>() ?? new List<PerformanceEvaluationDto>();
        }

        public async Task<PerformanceEvaluationDto?> GetPerformanceEvaluationByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PerformanceEvaluationDto>();
        }

        public async Task<PerformanceEvaluationDto> CreatePerformanceEvaluationAsync(CreatePerformanceEvaluationRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PerformanceEvaluationDto>() ?? throw new InvalidOperationException("Could not create performance evaluation.");
        }

        public async Task<PerformanceEvaluationDto?> UpdatePerformanceEvaluationAsync(int id, UpdatePerformanceEvaluationRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PerformanceEvaluationDto>();
        }

        public async Task<bool> DeletePerformanceEvaluationAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
