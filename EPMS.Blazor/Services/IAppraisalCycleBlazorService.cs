using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services
{
    public interface IAppraisalCycleBlazorService
    {
        Task<IEnumerable<AppraisalCycleDto>> GetAllAppraisalCyclesAsync();
        Task<AppraisalCycleDto?> GetAppraisalCycleByIdAsync(int id);
        Task<AppraisalCycleDto> CreateAppraisalCycleAsync(CreateAppraisalCycleRequest request);
        Task<AppraisalCycleDto?> UpdateAppraisalCycleAsync(int id, UpdateAppraisalCycleRequest request);
        Task<bool> DeleteAppraisalCycleAsync(int id);
    }
}
