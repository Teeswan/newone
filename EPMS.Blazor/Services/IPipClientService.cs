using EPMS.Domain.SpResults;
using EPMS.Shared.Responses;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services
{
    public interface IPipClientService
    {
        Task<ApiResponse<IEnumerable<PipPlanDto>>?> GetAllAsync();
        Task<ApiResponse<PipPlanDto>?> GetByIdAsync(int pipId);
        Task<ApiResponse<IEnumerable<PipPlanDto>>?> GetByEmployeeAsync(int employeeId);
        Task<ApiResponse<PipPlanDto>?> CreatePipAsync(CreatePipPlanRequest request);
        Task<ApiResponse<PipPlanDto>?> UpdatePipAsync(UpdatePipPlanRequest request);
        Task<ApiResponse<bool>?> DeletePipAsync(int pipId);

        Task<ApiResponse<PipObjectiveDto>?> AddObjectiveAsync(CreatePipObjectiveRequest request);
        Task<ApiResponse<PipObjectiveDto>?> UpdateObjectiveAsync(UpdatePipObjectiveRequest request);
        Task<ApiResponse<bool>?> DeleteObjectiveAsync(int objectiveId);

        Task<ApiResponse<PipMeetingDto>?> AddMeetingAsync(CreatePipMeetingRequest request);
        Task<ApiResponse<PipMeetingDto>?> UpdateMeetingAsync(UpdatePipMeetingRequest request);
        Task<ApiResponse<bool>?> DeleteMeetingAsync(int meetingId);

        Task<ApiResponse<IEnumerable<PipReportResult>>?> GetSummaryReportAsync(
            int? managerId = null, string? status = null);
    }
}
