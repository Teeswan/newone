using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services
{
    public class ImportResponse
    {
        public int Imported { get; set; }
    }

    public class DepartmentBlazorService : IDepartmentBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/Departments";

        public DepartmentBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<DepartmentDto>>() ?? new List<DepartmentDto>();
        }

        public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DepartmentDto>();
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DepartmentDto>() ?? throw new InvalidOperationException("Could not create department.");
        }

        public async Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DepartmentDto>();
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<DepartmentTreeDto>> GetDepartmentTreeAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/tree");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<DepartmentTreeDto>>() ?? new List<DepartmentTreeDto>();
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
            content.Add(fileContent, "file", "Departments.xlsx");
            
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
