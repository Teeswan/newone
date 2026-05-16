using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services
{
    public class AppraisalQuestionBlazorService : IAppraisalQuestionBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/AppraisalQuestions";

        public AppraisalQuestionBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<AppraisalQuestionDto>> GetAllAppraisalQuestionsAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<AppraisalQuestionDto>>() ?? new List<AppraisalQuestionDto>();
        }

        public async Task<AppraisalQuestionDto?> GetAppraisalQuestionByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppraisalQuestionDto>();
        }

        public async Task<AppraisalQuestionDto> CreateAppraisalQuestionAsync(CreateAppraisalQuestionRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppraisalQuestionDto>() ?? throw new InvalidOperationException("Could not create appraisal question.");
        }

        public async Task<AppraisalQuestionDto?> UpdateAppraisalQuestionAsync(int id, UpdateAppraisalQuestionRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppraisalQuestionDto>();
        }

        public async Task<bool> DeleteAppraisalQuestionAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
