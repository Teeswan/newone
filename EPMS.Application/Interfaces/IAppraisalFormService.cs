using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

public interface IAppraisalFormService
{
    Task<IEnumerable<AppraisalFormDto>> GetAllAsync();
    Task<AppraisalFormDto?> GetByIdAsync(int formId);
    Task<AppraisalFormDto> CreateAsync(CreateAppraisalFormRequest request);
    Task<AppraisalFormDto?> UpdateAsync(int formId, UpdateAppraisalFormRequest request);
    Task<bool> DeleteAsync(int formId);
}
