using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using EPMS.Shared.Responses;
using EPMS.Domain.SpResults;

namespace EPMS.Application.Interfaces
{
    public interface IPipService
    {
        // Plans
        Task<ApiResponse<PipPlanDto>> GetPipByIdAsync(int pipId, CancellationToken ct = default);
        Task<ApiResponse<IEnumerable<PipPlanDto>>> GetAllPipsAsync(CancellationToken ct = default);
        Task<ApiResponse<IEnumerable<PipPlanDto>>> GetPipsByEmployeeAsync(int employeeId, CancellationToken ct = default);
        Task<ApiResponse<IEnumerable<PipPlanDto>>> GetPipsByManagerAsync(int managerId, CancellationToken ct = default);
        Task<ApiResponse<PipPlanDto>> CreatePipAsync(CreatePipPlanRequest request, CancellationToken ct = default);
        Task<ApiResponse<PipPlanDto>> UpdatePipAsync(UpdatePipPlanRequest request, CancellationToken ct = default);
        Task<ApiResponse<bool>> DeletePipAsync(int pipId, CancellationToken ct = default);

        // Objectives
        Task<ApiResponse<PipObjectiveDto>> AddObjectiveAsync(CreatePipObjectiveRequest request, CancellationToken ct = default);
        Task<ApiResponse<PipObjectiveDto>> UpdateObjectiveAsync(UpdatePipObjectiveRequest request, CancellationToken ct = default);
        Task<ApiResponse<bool>> DeleteObjectiveAsync(int objectiveId, CancellationToken ct = default);

        // Meetings
        Task<ApiResponse<PipMeetingDto>> AddMeetingAsync(CreatePipMeetingRequest request, CancellationToken ct = default);
        Task<ApiResponse<PipMeetingDto>> UpdateMeetingAsync(UpdatePipMeetingRequest request, CancellationToken ct = default);
        Task<ApiResponse<bool>> DeleteMeetingAsync(int meetingId, CancellationToken ct = default);

        // Reports (Dapper)
        Task<ApiResponse<IEnumerable<PipReportResult>>> GetSummaryReportAsync(
            int? managerId = null, string? status = null, CancellationToken ct = default);
    }
}
