using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services
{
    public class AppraisalCycleBlazorService : IAppraisalCycleBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/AppraisalCycles"; // Assuming API base URL

        public AppraisalCycleBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<AppraisalCycleDto>> GetAllAppraisalCyclesAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<AppraisalCycleDto>>() ?? new List<AppraisalCycleDto>();
        }

        public async Task<AppraisalCycleDto?> GetAppraisalCycleByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppraisalCycleDto>();
        }

        public async Task<AppraisalCycleDto> CreateAppraisalCycleAsync(CreateAppraisalCycleRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppraisalCycleDto>() ?? throw new InvalidOperationException("Could not create appraisal cycle.");
        }

        public async Task<AppraisalCycleDto?> UpdateAppraisalCycleAsync(int id, UpdateAppraisalCycleRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppraisalCycleDto>();
        }

        public async Task<bool> DeleteAppraisalCycleAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode; // Returns true if deletion was successful
        }
    }
}
