using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services
{
    public interface IPerformanceEvaluationBlazorService
    {
        Task<IEnumerable<PerformanceEvaluationDto>> GetAllPerformanceEvaluationsAsync();
        Task<PerformanceEvaluationDto?> GetPerformanceEvaluationByIdAsync(int id);
        Task<PerformanceEvaluationDto> CreatePerformanceEvaluationAsync(CreatePerformanceEvaluationRequest request);
        Task<PerformanceEvaluationDto?> UpdatePerformanceEvaluationAsync(int id, UpdatePerformanceEvaluationRequest request);
        Task<bool> DeletePerformanceEvaluationAsync(int id);
    }
}
