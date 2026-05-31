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

        public async Task<PerformanceEvaluationDto> UpdatePerformanceEvaluationAsync(int id, UpdatePerformanceEvaluationRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PerformanceEvaluationDto>() ?? throw new InvalidOperationException("Could not update performance evaluation.");
        }

        public async Task<bool> SubmitSelfAssessmentAsync(int id)
        {
            var response = await _httpClient.PostAsync($"{BaseUrl}/{id}/submit-self", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SubmitManagerReviewAsync(int id)
        {
            var response = await _httpClient.PostAsync($"{BaseUrl}/{id}/submit-manager", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> FinalizePerformanceEvaluationAsync(int id)
        {
            var response = await _httpClient.PostAsync($"{BaseUrl}/{id}/finalize", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CompletePerformanceEvaluationAsync(int id)
        {
            var response = await _httpClient.PostAsync($"{BaseUrl}/{id}/complete", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReopenPerformanceEvaluationAsync(int id)
        {
            var response = await _httpClient.PostAsync($"{BaseUrl}/{id}/reopen", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePerformanceEvaluationAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<int> CreateBulkPerformanceEvaluationsAsync(BulkPerformanceEvaluationRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/bulk", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<IEnumerable<CalibrationTrendDto>> GetCalibrationTrendAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CalibrationTrendDto>>($"{BaseUrl}/calibration-trend") ?? new List<CalibrationTrendDto>();
        }

        public async Task<byte[]> Download360FeedbackRdlcAsync(int id)
        {
            return await _httpClient.GetByteArrayAsync($"{BaseUrl}/{id}/report/rdlc/360");
        }

        public async Task<byte[]> DownloadPerformanceAppraisalRdlcAsync(int id)
        {
            return await _httpClient.GetByteArrayAsync($"{BaseUrl}/{id}/report/rdlc/appraisal");
        }

        public async Task<byte[]> DownloadSelfAssessmentRdlcAsync(int id)
        {
            return await _httpClient.GetByteArrayAsync($"{BaseUrl}/{id}/report/rdlc/self-assessment");
        }
    }
}
