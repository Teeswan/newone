using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using EPMS.Shared.Common;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services
{
    public class TeamBlazorService : ITeamBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/Teams";

        public TeamBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<TeamDto>> GetAllTeamsAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<TeamDto>>>();
                return result?.Data ?? new List<TeamDto>();
            }
            return new List<TeamDto>();
        }

        public async Task<TeamDto?> GetTeamByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<TeamDto>>();
                return result?.Data;
            }
            return null;
        }

        public async Task<TeamDto> CreateTeamAsync(CreateTeamRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<TeamDto>>();
            
            if (response.IsSuccessStatusCode && result != null && result.Success)
            {
                return result.Data!;
            }
            
            throw new InvalidOperationException(result?.Message ?? "Could not create team.");
        }

        public async Task<TeamDto?> UpdateTeamAsync(int id, UpdateTeamRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<TeamDto>>();
            
            if (response.IsSuccessStatusCode && result != null && result.Success)
            {
                return result.Data;
            }
            
            throw new InvalidOperationException(result?.Message ?? "Could not update team.");
        }

        public async Task<bool> DeleteTeamAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<TeamDetailDto>> GetTeamsByDepartmentAsync(int departmentId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/department/{departmentId}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<TeamDetailDto>>>();
                return result?.Data ?? new List<TeamDetailDto>();
            }
            return new List<TeamDetailDto>();
        }

        public async Task<byte[]> ExportToExcelAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/export/excel");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> ExportToPdfAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/export/pdf");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<int> ImportFromExcelAsync(Stream fileStream, bool skipFirstRow = true, string sheetName = "", bool skipExisting = false)
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            content.Add(fileContent, "file", "Teams.xlsx");
            
            var queryParams = new System.Collections.Generic.List<string>();
            queryParams.Add($"skipFirstRow={skipFirstRow}");
            if (!string.IsNullOrWhiteSpace(sheetName))
            {
                queryParams.Add($"sheetName={Uri.EscapeDataString(sheetName)}");
            }
            if (skipExisting)
            {
                queryParams.Add($"skipExisting=true");
            }
            
            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.PostAsync($"{BaseUrl}/import/excel?{queryString}", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ArgumentException(errorMessage);
            }
            
            var result = await response.Content.ReadFromJsonAsync<ImportResponse>();
            return result?.Imported ?? 0;
        }
    }
}
