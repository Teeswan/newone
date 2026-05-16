using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services
{
    public interface IAppraisalResponseBlazorService
    {
        Task<IEnumerable<AppraisalResponseDto>> GetAllAppraisalResponsesAsync();
        Task<AppraisalResponseDto?> GetAppraisalResponseByIdAsync(int id);
        Task<AppraisalResponseDto> CreateAppraisalResponseAsync(CreateAppraisalResponseRequest request);
        Task<AppraisalResponseDto?> UpdateAppraisalResponseAsync(int id, UpdateAppraisalResponseRequest request);
        Task<bool> DeleteAppraisalResponseAsync(int id);
    }
}
