using EPMS.Domain.SpResults;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using EPMS.Shared.Responses;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services
{
    public sealed class PipClientService : IPipClientService
    {
        private readonly HttpClient _http;
        private const string Base = "api/pip";

        public PipClientService(HttpClient http) => _http = http;

        public Task<ApiResponse<IEnumerable<PipPlanDto>>?> GetAllAsync() =>
            _http.GetFromJsonAsync<ApiResponse<IEnumerable<PipPlanDto>>>(Base);

        public Task<ApiResponse<PipPlanDto>?> GetByIdAsync(int pipId) =>
            _http.GetFromJsonAsync<ApiResponse<PipPlanDto>>($"{Base}/{pipId}");

        public Task<ApiResponse<IEnumerable<PipPlanDto>>?> GetByEmployeeAsync(int employeeId) =>
            _http.GetFromJsonAsync<ApiResponse<IEnumerable<PipPlanDto>>>($"{Base}/employee/{employeeId}");

        public async Task<ApiResponse<PipPlanDto>?> CreatePipAsync(CreatePipPlanRequest request)
        {
            var response = await _http.PostAsJsonAsync(Base, request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResponse<PipPlanDto>.Fail($"HTTP Error: {response.StatusCode} - {errorContent}", new List<string> { errorContent });
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<PipPlanDto>>();
        }

        public async Task<ApiResponse<PipPlanDto>?> UpdatePipAsync(UpdatePipPlanRequest request)
        {
            var response = await _http.PutAsJsonAsync(Base, request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResponse<PipPlanDto>.Fail($"HTTP Error: {response.StatusCode} - {errorContent}", new List<string> { errorContent });
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<PipPlanDto>>();
        }

        public async Task<ApiResponse<bool>?> DeletePipAsync(int pipId)
        {
            var response = await _http.DeleteAsync($"{Base}/{pipId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResponse<bool>.Fail($"HTTP Error: {response.StatusCode} - {errorContent}", new List<string> { errorContent });
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        }

        public async Task<ApiResponse<PipObjectiveDto>?> AddObjectiveAsync(CreatePipObjectiveRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{Base}/objectives", request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResponse<PipObjectiveDto>.Fail($"HTTP Error: {response.StatusCode} - {errorContent}", new List<string> { errorContent });
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<PipObjectiveDto>>();
        }

        public async Task<ApiResponse<PipObjectiveDto>?> UpdateObjectiveAsync(UpdatePipObjectiveRequest request)
        {
            var response = await _http.PutAsJsonAsync($"{Base}/objectives", request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResponse<PipObjectiveDto>.Fail($"HTTP Error: {response.StatusCode} - {errorContent}", new List<string> { errorContent });
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<PipObjectiveDto>>();
        }

        public async Task<ApiResponse<bool>?> DeleteObjectiveAsync(int objectiveId)
        {
            var response = await _http.DeleteAsync($"{Base}/objectives/{objectiveId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResponse<bool>.Fail($"HTTP Error: {response.StatusCode} - {errorContent}", new List<string> { errorContent });
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        }

        public async Task<ApiResponse<PipMeetingDto>?> AddMeetingAsync(CreatePipMeetingRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{Base}/meetings", request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResponse<PipMeetingDto>.Fail($"HTTP Error: {response.StatusCode} - {errorContent}", new List<string> { errorContent });
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<PipMeetingDto>>();
        }

        public async Task<ApiResponse<PipMeetingDto>?> UpdateMeetingAsync(UpdatePipMeetingRequest request)
        {
            var response = await _http.PutAsJsonAsync($"{Base}/meetings", request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResponse<PipMeetingDto>.Fail($"HTTP Error: {response.StatusCode} - {errorContent}", new List<string> { errorContent });
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<PipMeetingDto>>();
        }

        public async Task<ApiResponse<bool>?> DeleteMeetingAsync(int meetingId)
        {
            var response = await _http.DeleteAsync($"{Base}/meetings/{meetingId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResponse<bool>.Fail($"HTTP Error: {response.StatusCode} - {errorContent}", new List<string> { errorContent });
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        }

        public Task<ApiResponse<IEnumerable<PipReportResult>>?> GetSummaryReportAsync(
            int? managerId = null, string? status = null)
        {
            var query = $"{Base}/reports/summary?managerId={managerId}&status={status}";
            return _http.GetFromJsonAsync<ApiResponse<IEnumerable<PipReportResult>>>(query);
        }
    }
}
