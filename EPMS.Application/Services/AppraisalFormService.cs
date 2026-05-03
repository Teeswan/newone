using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class AppraisalFormService : IAppraisalFormService
{
    private readonly IAppraisalFormRepository _repository;
    private readonly IMapper _mapper;

    public AppraisalFormService(IAppraisalFormRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AppraisalFormDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<AppraisalFormDto>>(entities);
    }

    public async Task<AppraisalFormDto?> GetByIdAsync(int formId)
    {
        var entity = await _repository.GetByIdAsync(formId);
        return _mapper.Map<AppraisalFormDto?>(entity);
    }

    public async Task<AppraisalFormDto> CreateAsync(CreateAppraisalFormRequest request)
    {
        var entity = _mapper.Map<ApplicationForm>(request);
        var created = await _repository.CreateAsync(entity);
        return _mapper.Map<AppraisalFormDto>(created);
    }

    public async Task<AppraisalFormDto?> UpdateAsync(int formId, UpdateAppraisalFormRequest request)
    {
        var entity = _mapper.Map<ApplicationForm>(request);
        entity.FormId = formId;

        var updated = await _repository.UpdateAsync(entity);
        return _mapper.Map<AppraisalFormDto?>(updated);
    }

    public async Task<bool> DeleteAsync(int formId)
    {
        return await _repository.DeleteAsync(formId);
    }
}
