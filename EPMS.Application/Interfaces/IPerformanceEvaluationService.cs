using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

public interface IPerformanceEvaluationService
{
    Task<IEnumerable<PerformanceEvaluationDto>> GetAllAsync();
    Task<PerformanceEvaluationDto?> GetByIdAsync(int evalId);
    Task<IEnumerable<PerformanceEvaluationDto>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<PerformanceEvaluationDto>> GetByCycleIdAsync(int cycleId);
    Task<PerformanceEvaluationDto> CreateAsync(CreatePerformanceEvaluationRequest request);
    Task<PerformanceEvaluationDto?> UpdateAsync(int evalId, UpdatePerformanceEvaluationRequest request);
    Task<bool> SubmitSelfAssessmentAsync(int evalId);
    Task<bool> DeleteAsync(int evalId);
}
