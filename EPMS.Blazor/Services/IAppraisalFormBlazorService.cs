using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services
{
    public interface IAppraisalFormBlazorService
    {
        Task<IEnumerable<AppraisalFormDto>> GetAllAppraisalFormsAsync();
        Task<AppraisalFormDto?> GetAppraisalFormByIdAsync(int id);
        Task<AppraisalFormDto> CreateAppraisalFormAsync(CreateAppraisalFormRequest request);
        Task<AppraisalFormDto?> UpdateAppraisalFormAsync(int id, UpdateAppraisalFormRequest request);
        Task<bool> DeleteAppraisalFormAsync(int id);
    }
}
