using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

public interface IAppraisalCycleService
{
    Task<IEnumerable<AppraisalCycleDto>> GetAllAsync();
    Task<AppraisalCycleDto?> GetByIdAsync(int cycleId);
    Task<AppraisalCycleDto> CreateAsync(CreateAppraisalCycleRequest request);
    Task<AppraisalCycleDto?> UpdateAsync(int cycleId, UpdateAppraisalCycleRequest request);
    Task<bool> DeleteAsync(int cycleId);
}
