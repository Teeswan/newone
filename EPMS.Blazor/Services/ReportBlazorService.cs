using System.Net.Http.Json;
using EPMS.Shared.DTOs;

namespace EPMS.Blazor.Services;

public class ReportBlazorService : IReportBlazorService
{
    private readonly HttpClient _httpClient;

    public ReportBlazorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<byte[]> GenerateEmployeePerformanceReportAsync(EmployeePerformanceReportDto reportData)
    {
        var response = await _httpClient.PostAsJsonAsync("api/reports/employee-performance", reportData);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<byte[]> GetSampleEmployeePerformanceReportAsync()
    {
        var response = await _httpClient.GetAsync("api/reports/test-sample");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }
}
