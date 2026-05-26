using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using EPMS.Shared.DTOs;

namespace EPMS.Blazor.Services;

public class ReportBlazorService : IReportBlazorService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public ReportBlazorService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<byte[]> GenerateEmployeePerformanceSummaryReportAsync(EmployeePerformanceSummaryReportDto reportData)
    {
        var response = await _httpClient.PostAsJsonAsync("api/reports/employee-performance-summary", reportData);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<byte[]> GenerateDepartmentPerformanceComparisonReportAsync()
    {
        var response = await _httpClient.GetAsync("api/reports/department-performance-comparison");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<byte[]> GenerateHighLowPerformerReportAsync()
    {
        var response = await _httpClient.GetAsync("api/reports/high-low-performers");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<byte[]> GeneratePromotionIncrementRecommendationReportAsync()
    {
        var response = await _httpClient.GetAsync("api/reports/promotion-increment-recommendations");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<byte[]> Generate360FeedbackRdlcAsync(int evalId)
    {
        var response = await _httpClient.GetAsync($"api/PerformanceEvaluations/{evalId}/report/rdlc/360");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<byte[]> GeneratePerformanceAppraisalRdlcAsync(int evalId)
    {
        var response = await _httpClient.GetAsync($"api/PerformanceEvaluations/{evalId}/report/rdlc/appraisal");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<byte[]> GenerateSelfAssessmentRdlcAsync(int evalId)
    {
        var response = await _httpClient.GetAsync($"api/PerformanceEvaluations/{evalId}/report/rdlc/self-assessment");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task DownloadPdfAsync(byte[] pdfBytes, string fileName)
    {
        var fileStream = new MemoryStream(pdfBytes);
        var fileNameSafe = fileName.Replace(" ", "_");
        var dotNetReference = DotNetObjectReference.Create(this);
        
        await _jsRuntime.InvokeVoidAsync(
            "downloadFile",
            fileNameSafe,
            Convert.ToBase64String(pdfBytes));
    }

    public async Task OpenPdfInNewTabAsync(byte[] pdfBytes)
    {
        var base64 = Convert.ToBase64String(pdfBytes);
        var dataUrl = $"data:application/pdf;base64,{base64}";
        
        await _jsRuntime.InvokeVoidAsync("openInNewTab", dataUrl);
    }
}