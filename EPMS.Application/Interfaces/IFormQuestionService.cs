using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

public interface IFormQuestionService
{
    Task<IEnumerable<FormQuestionDto>> GetAllAsync();
    Task<IEnumerable<FormQuestionDto>> GetByFormIdAsync(int formId);
    Task<FormQuestionDto?> GetByFormAndQuestionIdAsync(int formId, int questionId);
    Task<FormQuestionDto> CreateAsync(CreateFormQuestionRequest request);
    Task<FormQuestionDto?> UpdateAsync(int formId, int questionId, UpdateFormQuestionRequest request);
    Task<bool> DeleteAsync(int formId, int questionId);
}
