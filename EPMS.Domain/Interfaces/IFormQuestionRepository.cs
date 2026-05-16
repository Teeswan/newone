using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IFormQuestionRepository
{
    Task<IEnumerable<FormQuestion>> GetAllAsync();
    Task<IEnumerable<FormQuestion>> GetByFormIdAsync(int formId);
    Task<FormQuestion?> GetByFormAndQuestionIdAsync(int formId, int questionId);
    Task<FormQuestion> CreateAsync(FormQuestion entity);
    Task<FormQuestion?> UpdateAsync(FormQuestion entity);
    Task<bool> DeleteAsync(int formId, int questionId);
}
