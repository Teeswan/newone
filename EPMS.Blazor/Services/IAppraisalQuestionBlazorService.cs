using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services
{
    public interface IAppraisalQuestionBlazorService
    {
        Task<IEnumerable<AppraisalQuestionDto>> GetAllAppraisalQuestionsAsync();
        Task<AppraisalQuestionDto?> GetAppraisalQuestionByIdAsync(int id);
        Task<AppraisalQuestionDto> CreateAppraisalQuestionAsync(CreateAppraisalQuestionRequest request);
        Task<AppraisalQuestionDto?> UpdateAppraisalQuestionAsync(int id, UpdateAppraisalQuestionRequest request);
        Task<bool> DeleteAppraisalQuestionAsync(int id);
    }
}
