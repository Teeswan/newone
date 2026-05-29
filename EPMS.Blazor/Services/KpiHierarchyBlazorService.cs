using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services
{
    public class KpiHierarchyBlazorService : IKpiHierarchyBlazorService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/KpiHierarchy";

        public KpiHierarchyBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<KpiDto>> GetAllMasterAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<KpiDto>>($"{BaseUrl}/master") ?? new List<KpiDto>();
        }

        public async Task<KpiDto?> GetMasterByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<KpiDto>($"{BaseUrl}/master/{id}");
        }

        public async Task<KpiDto> CreateMasterAsync(KpiRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/master", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<KpiDto>() ?? throw new InvalidOperationException();
        }

        public async Task<KpiDto?> UpdateMasterAsync(int id, KpiRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/master/{id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<KpiDto>();
        }

        public async Task<bool> DeleteMasterAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/master/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<DepartmentKpiDto>> GetAllDeptAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DepartmentKpiDto>>($"{BaseUrl}/department") ?? new List<DepartmentKpiDto>();
        }

        public async Task<IEnumerable<DepartmentKpiDto>> GetDeptByDeptAsync(int deptId, int cycleId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DepartmentKpiDto>>($"{BaseUrl}/department/by-dept/{deptId}/{cycleId}") ?? new List<DepartmentKpiDto>();
        }

        public async Task<DepartmentKpiDto> CreateDeptAsync(DepartmentKpiRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/department", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DepartmentKpiDto>() ?? throw new InvalidOperationException();
        }

        public async Task<IEnumerable<TeamKpiDto>> GetAllTeamAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<TeamKpiDto>>($"{BaseUrl}/team") ?? new List<TeamKpiDto>();
        }

        public async Task<IEnumerable<TeamKpiDto>> GetTeamByTeamAsync(int teamId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<TeamKpiDto>>($"{BaseUrl}/team/by-team/{teamId}") ?? new List<TeamKpiDto>();
        }

        public async Task<TeamKpiDto> CreateTeamAsync(TeamKpiRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/team", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TeamKpiDto>() ?? throw new InvalidOperationException();
        }

        public async Task<IEnumerable<EmployeeKpiDto>> GetAllEmpAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<EmployeeKpiDto>>($"{BaseUrl}/employee") ?? new List<EmployeeKpiDto>();
        }

        public async Task<IEnumerable<EmployeeKpiDto>> GetEmpByEmpAsync(int empId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<EmployeeKpiDto>>($"{BaseUrl}/employee/by-emp/{empId}") ?? new List<EmployeeKpiDto>();
        }

        public async Task<EmployeeKpiDto> CreateEmpAsync(EmployeeKpiRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/employee", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EmployeeKpiDto>() ?? throw new InvalidOperationException();
        }

        public async Task<EmployeeKpiDto?> UpdateEmpAsync(int id, EmployeeKpiRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/employee/{id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EmployeeKpiDto>();
        }
    }
}
