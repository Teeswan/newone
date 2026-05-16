using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services
{
    public interface IFormQuestionBlazorService
    {
        Task<IEnumerable<FormQuestionDto>> GetAllFormQuestionsAsync();
        Task<FormQuestionDto?> GetFormQuestionByIdsAsync(int formId, int questionId);
        Task<FormQuestionDto> CreateFormQuestionAsync(CreateFormQuestionRequest request);
        Task<bool> DeleteFormQuestionAsync(int formId, int questionId);
    }
}
