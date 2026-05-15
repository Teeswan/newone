using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

public interface IAppraisalResponseService
{
    Task<IEnumerable<AppraisalResponseDto>> GetAllAsync();
    Task<AppraisalResponseDto?> GetByIdAsync(long responseId);
    Task<IEnumerable<AppraisalResponseDto>> GetByEvalIdAsync(int evalId, int? currentEmployeeId = null);
    Task<AppraisalResponseDto> CreateAsync(CreateAppraisalResponseRequest request);
    Task<AppraisalResponseDto?> UpdateAsync(long responseId, UpdateAppraisalResponseRequest request);
    Task<bool> DeleteAsync(long responseId);
}
