using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EPMS.Blazor.Services
{
    public class EmployeeKpiBlazorService : IEmployeeKpiBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/EmployeeKpis";

        public EmployeeKpiBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<EmployeeKpiDto>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<EmployeeKpiDto>>(BaseUrl) ?? new List<EmployeeKpiDto>();
        }

        public async Task<EmployeeKpiDto?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<EmployeeKpiDto>($"{BaseUrl}/{id}");
        }

        public async Task<IEnumerable<EmployeeKpiDto>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<EmployeeKpiDto>>($"{BaseUrl}/employee/{employeeId}") ?? new List<EmployeeKpiDto>();
        }

        public async Task<EmployeeKpiDto> CreateAsync(EmployeeKpiRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return await response.Content.ReadFromJsonAsync<EmployeeKpiDto>() ?? throw new InvalidOperationException();
        }

        public async Task<IEnumerable<EmployeeKpiDto>> CreateBulkAsync(BulkEmployeeKpiRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/bulk", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<EmployeeKpiDto>>() ?? new List<EmployeeKpiDto>();
        }

        public async Task<EmployeeKpiDto?> UpdateAsync(int id, EmployeeKpiRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return await response.Content.ReadFromJsonAsync<EmployeeKpiDto>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<EmployeeKpiDto>> CalculateForEmployeeAsync(int employeeId)
        {
            var response = await _httpClient.PostAsync($"{BaseUrl}/calculate/{employeeId}", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return await response.Content.ReadFromJsonAsync<IEnumerable<EmployeeKpiDto>>() ?? new List<EmployeeKpiDto>();
        }
    }
}
