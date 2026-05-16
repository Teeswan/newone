using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

public interface IAppraisalQuestionService
{
    Task<IEnumerable<AppraisalQuestionDto>> GetAllAsync();
    Task<AppraisalQuestionDto?> GetByIdAsync(int questionId);
    Task<AppraisalQuestionDto> CreateAsync(CreateAppraisalQuestionRequest request);
    Task<AppraisalQuestionDto?> UpdateAsync(int questionId, UpdateAppraisalQuestionRequest request);
    Task<bool> DeleteAsync(int questionId);
}
