using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services
{
    public class AppraisalFormBlazorService : IAppraisalFormBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/AppraisalForms";

        public AppraisalFormBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<AppraisalFormDto>> GetAllAppraisalFormsAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<AppraisalFormDto>>() ?? new List<AppraisalFormDto>();
        }

        public async Task<AppraisalFormDto?> GetAppraisalFormByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppraisalFormDto>();
        }

        public async Task<AppraisalFormDto> CreateAppraisalFormAsync(CreateAppraisalFormRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppraisalFormDto>() ?? throw new InvalidOperationException("Could not create appraisal form.");
        }

        public async Task<AppraisalFormDto?> UpdateAppraisalFormAsync(int id, UpdateAppraisalFormRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppraisalFormDto>();
        }

        public async Task<bool> DeleteAppraisalFormAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
