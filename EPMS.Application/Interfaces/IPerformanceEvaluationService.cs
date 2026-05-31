using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

public interface IPerformanceEvaluationService
{
    Task<IEnumerable<PerformanceEvaluationDto>> GetAllAsync();
    Task<PerformanceEvaluationDto?> GetByIdAsync(int evalId, int? currentEmployeeId = null);
    Task<IEnumerable<PerformanceEvaluationDto>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<PerformanceEvaluationDto>> GetByCycleIdAsync(int cycleId);
    Task<PerformanceEvaluationDto> CreateAsync(CreatePerformanceEvaluationRequest request, int? currentEmployeeId = null);
    Task<int> CreateBulkAsync(BulkPerformanceEvaluationRequest request, int? currentEmployeeId = null);
    Task<PerformanceEvaluationDto?> UpdateAsync(int evalId, UpdatePerformanceEvaluationRequest request, int? currentEmployeeId = null);
    Task<bool> SubmitSelfAssessmentAsync(int evalId, int? currentEmployeeId = null);
    Task<bool> SubmitManagerReviewAsync(int evalId, int? currentEmployeeId = null);
    Task<bool> FinalizeAsync(int evalId, int? currentEmployeeId = null);
    Task<bool> ReopenAsync(int evalId, int? currentEmployeeId = null);
    Task<bool> DeleteAsync(int evalId);
    Task<AppraisalReportDto?> GetAppraisalReportDataAsync(int evalId);
    Task<IEnumerable<CalibrationTrendDto>> GetCalibrationTrendAsync();
}
