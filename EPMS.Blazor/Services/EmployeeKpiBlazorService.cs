using EPMS.Shared.DTOs;
using EPMS.Shared.Common;
using System.Net.Http.Json;
using EPMS.Application.UseCases.KpiAssignment.Commands;

namespace EPMS.Blazor.Services
{
    public interface IEmployeeKpiBlazorService
    {
        Task<IEnumerable<EmployeeKpiAssignmentDto>> GetAssignmentsAsync(int employeeId, int cycleId);
        Task<KpiScoreSummaryDto?> GetScoreSummaryAsync(int employeeId, int cycleId);
        Task<bool> AddAdHocAsync(AddAdHocKpiCommand command);
        Task<bool> FinaliseAsync(FinaliseKpiAssignmentCommand command);
        Task<KpiScoreSummaryDto?> EnterActualAsync(EnterActualValueCommand command);
        Task<bool> ActivateCycleSnapshotAsync(ActivateCycleKpiSnapshotCommand command);
        Task<BulkImportResultDto?> BulkImportExcelAsync(Stream fileStream, string fileName);
        Task<byte[]> DownloadTemplateAsync();
    }

    public class EmployeeKpiBlazorService : IEmployeeKpiBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/employee-kpi";

        public EmployeeKpiBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<EmployeeKpiAssignmentDto>> GetAssignmentsAsync(int employeeId, int cycleId)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<EmployeeKpiAssignmentDto>>>($"{BaseUrl}/assignments/{employeeId}/{cycleId}");
            return response?.Data ?? Enumerable.Empty<EmployeeKpiAssignmentDto>();
        }

        public async Task<KpiScoreSummaryDto?> GetScoreSummaryAsync(int employeeId, int cycleId)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<KpiScoreSummaryDto>>($"{BaseUrl}/score-summary/{employeeId}/{cycleId}");
            return response?.Data;
        }

        public async Task<bool> AddAdHocAsync(AddAdHocKpiCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/ad-hoc", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> FinaliseAsync(FinaliseKpiAssignmentCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/finalise", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<KpiScoreSummaryDto?> EnterActualAsync(EnterActualValueCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/enter-actual", command);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<KpiScoreSummaryDto>>();
            return result?.Data;
        }

        public async Task<bool> ActivateCycleSnapshotAsync(ActivateCycleKpiSnapshotCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/activate-cycle-snapshot", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<BulkImportResultDto?> BulkImportExcelAsync(Stream fileStream, string fileName)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(fileStream);
            content.Add(fileContent, "file", fileName);

            var response = await _httpClient.PostAsync($"{BaseUrl}/excel-bulk-import", content);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<BulkImportResultDto>>();
            return result?.Data;
        }

        public async Task<byte[]> DownloadTemplateAsync()
        {
            return await _httpClient.GetByteArrayAsync($"{BaseUrl}/import-template");
        }
    }
}
