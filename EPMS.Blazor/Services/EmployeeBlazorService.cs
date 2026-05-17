using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace EPMS.Blazor.Services
{
    public class EmployeeBlazorService : IEmployeeBlazorService
    {
        private readonly HttpClient _httpClient;

        public EmployeeBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EmployeeDto>> GetEmployeesAsync()
        {
         
            var response = await _httpClient.GetFromJsonAsync<List<EmployeeDto>>("api/employees");
            return response ?? new List<EmployeeDto>();
        }

        public async Task<EmployeeDetailDto?> GetEmployeeDetailsAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<EmployeeDetailDto>($"api/employees/{id}");
        }

        public async Task<List<EmployeeDto>> GetHierarchyAsync()
        {
            // Hierarchy အတွက် Backend Endpoint
            var response = await _httpClient.GetFromJsonAsync<List<EmployeeDto>>("api/employees/hierarchy");
            return response ?? new List<EmployeeDto>();
        }

        public async Task<bool> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/employees", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception(errorContent ?? "Failed to create employee");
            }
            
            return true;
        }

        public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/employees/{id}", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception(errorContent ?? "Failed to update employee");
            }

            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/employees/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> IsEmployeeCodeUniqueAsync(string code)
        {
            var response = await _httpClient.GetAsync($"api/employees/code/{code}");
            return response.StatusCode == System.Net.HttpStatusCode.NotFound;
        }

        public async Task<byte[]> ExportToExcelAsync()
        {
            var response = await _httpClient.GetAsync("api/employees/export/excel");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> ExportToPdfAsync()
        {
            var response = await _httpClient.GetAsync("api/employees/export/pdf");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<int> ImportFromExcelAsync(Stream fileStream, string fileName)
        {
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            content.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync("api/employees/import/excel", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }

            var resultStr = await response.Content.ReadAsStringAsync();
            return int.Parse(resultStr);
        }
    }
}
