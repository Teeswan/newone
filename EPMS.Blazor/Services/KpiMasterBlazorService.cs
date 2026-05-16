using EPMS.Shared.DTOs;
using EPMS.Shared.Common;
using System.Net.Http.Json;
using EPMS.Application.UseCases.KpiMaster.Commands;

namespace EPMS.Blazor.Services
{
    public interface IKpiMasterBlazorService
    {
        Task<IEnumerable<KpiMasterDto>> GetKpiMastersAsync();
        Task<KpiMasterDto?> GetByIdAsync(int id);
        Task<IEnumerable<KpiMasterDto>> GetByPositionAsync(int positionId);
        Task<int> CreateAsync(CreateKpiMasterCommand command);
        Task<bool> UpdateAsync(int id, UpdateKpiMasterCommand command);
        Task<bool> DeactivateAsync(int id);
        Task<BulkImportResultDto?> BulkImportExcelAsync(Stream fileStream, string fileName);
        Task<byte[]> DownloadTemplateAsync();
    }

    public class KpiMasterBlazorService : IKpiMasterBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/kpi-master";

        public KpiMasterBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<KpiMasterDto>> GetKpiMastersAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<KpiMasterDto>>>(BaseUrl);
            return response?.Data ?? Enumerable.Empty<KpiMasterDto>();
        }

        public async Task<KpiMasterDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<KpiMasterDto>>($"{BaseUrl}/{id}");
            return response?.Data;
        }

        public async Task<IEnumerable<KpiMasterDto>> GetByPositionAsync(int positionId)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<KpiMasterDto>>>(BaseUrl + "/by-position/" + positionId);
            return response?.Data ?? Enumerable.Empty<KpiMasterDto>();
        }

        public async Task<int> CreateAsync(CreateKpiMasterCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, command);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();
            return result?.Data ?? 0;
        }

        public async Task<bool> UpdateAsync(int id, UpdateKpiMasterCommand command)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
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
