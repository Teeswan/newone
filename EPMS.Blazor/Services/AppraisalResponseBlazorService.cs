using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services
{
    public class AppraisalResponseBlazorService : IAppraisalResponseBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/AppraisalResponses";

        public AppraisalResponseBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<AppraisalResponseDto>> GetAllAppraisalResponsesAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<AppraisalResponseDto>>() ?? new List<AppraisalResponseDto>();
        }

        public async Task<AppraisalResponseDto?> GetAppraisalResponseByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppraisalResponseDto>();
        }

        public async Task<AppraisalResponseDto> CreateAppraisalResponseAsync(CreateAppraisalResponseRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppraisalResponseDto>() ?? throw new InvalidOperationException("Could not create appraisal response.");
        }

        public async Task<AppraisalResponseDto?> UpdateAppraisalResponseAsync(int id, UpdateAppraisalResponseRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppraisalResponseDto>();
        }

        public async Task<bool> DeleteAppraisalResponseAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
