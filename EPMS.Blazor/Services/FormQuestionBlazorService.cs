using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services
{
    public class FormQuestionBlazorService : IFormQuestionBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/FormQuestions";

        public FormQuestionBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<FormQuestionDto>> GetAllFormQuestionsAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<FormQuestionDto>>() ?? new List<FormQuestionDto>();
        }

        public async Task<FormQuestionDto?> GetFormQuestionByIdsAsync(int formId, int questionId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{formId}/{questionId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FormQuestionDto>();
        }

        public async Task<FormQuestionDto> CreateFormQuestionAsync(CreateFormQuestionRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FormQuestionDto>() ?? throw new InvalidOperationException("Could not create form question.");
        }

        public async Task<bool> DeleteFormQuestionAsync(int formId, int questionId)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{formId}/{questionId}");
            return response.IsSuccessStatusCode;
        }
    }
}
