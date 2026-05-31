using EPMS.Shared.DTOs;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPMS.Blazor.Services;

public class AuditLogBlazorService : IAuditLogBlazorService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "api/AuditLogs";

    public AuditLogBlazorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<AuditLogDto>> GetAllAuditLogsAsync()
    {
        var response = await _httpClient.GetAsync(BaseUrl);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<AuditLogDto>>() ?? new List<AuditLogDto>();
    }

    public async Task<IEnumerable<AuditLogDto>> GetAuditLogsByEntityAsync(string entityName)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/entity/{entityName}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<AuditLogDto>>() ?? new List<AuditLogDto>();
    }
}
