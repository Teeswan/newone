using EPMS.Shared.DTOs;
using EPMS.Shared.Common;
using System.Net.Http.Json;
using EPMS.Application.UseCases.PositionKpi.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace EPMS.Blazor.Services
{
    public interface IPositionKpiBlazorService
    {
        Task<IEnumerable<PositionKpiDto>> GetPositionKpisAsync();
        Task<PositionKpiDto?> GetByIdAsync(int id);
        Task<IEnumerable<PositionKpiDto>> GetByPositionAsync(int positionId);
        Task<int> CreateAsync(CreatePositionKpiCommand command);
        Task<bool> UpdateAsync(int id, UpdatePositionKpiCommand command);
        Task<bool> DeactivateAsync(int id);
        Task<BulkImportResultDto?> BulkImportExcelAsync(Stream fileStream, string fileName);
        Task<byte[]> DownloadTemplateAsync();
    }

    public class PositionKpiBlazorService : IPositionKpiBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/position-kpi";

        public PositionKpiBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<PositionKpiDto>> GetPositionKpisAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<PositionKpiDto>>>(BaseUrl);
            return response?.Data ?? Enumerable.Empty<PositionKpiDto>();
        }

        public async Task<PositionKpiDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PositionKpiDto>>($"{BaseUrl}/{id}");
            return response?.Data;
        }

        public async Task<IEnumerable<PositionKpiDto>> GetByPositionAsync(int positionId)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<PositionKpiDto>>>(BaseUrl + "/by-position/" + positionId);
            return response?.Data ?? Enumerable.Empty<PositionKpiDto>();
        }

        public async Task<int> CreateAsync(CreatePositionKpiCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, command);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();
            return result?.Data ?? 0;
        }

        public async Task<bool> UpdateAsync(int id, UpdatePositionKpiCommand command)
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
