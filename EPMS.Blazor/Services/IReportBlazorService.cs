using EPMS.Shared.DTOs;

namespace EPMS.Blazor.Services;

public interface IReportBlazorService
{
    Task<byte[]> GenerateEmployeePerformanceReportAsync(EmployeePerformanceReportDto reportData);
    Task<byte[]> GetSampleEmployeePerformanceReportAsync();
}
