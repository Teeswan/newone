using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class AppraisalQuestionService : IAppraisalQuestionService
{
    private readonly IAppraisalQuestionRepository _repository;
    private readonly IMapper _mapper;

    public AppraisalQuestionService(IAppraisalQuestionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AppraisalQuestionDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<AppraisalQuestionDto>>(entities);
    }

    public async Task<AppraisalQuestionDto?> GetByIdAsync(int questionId)
    {
        var entity = await _repository.GetByIdAsync(questionId);
        return _mapper.Map<AppraisalQuestionDto?>(entity);
    }

    public async Task<AppraisalQuestionDto> CreateAsync(CreateAppraisalQuestionRequest request)
    {
        var entity = _mapper.Map<AppraisalQuestion>(request);
        var created = await _repository.CreateAsync(entity);
        return _mapper.Map<AppraisalQuestionDto>(created);
    }

    public async Task<AppraisalQuestionDto?> UpdateAsync(int questionId, UpdateAppraisalQuestionRequest request)
    {
        var entity = _mapper.Map<AppraisalQuestion>(request);
        entity.QuestionId = questionId;

        var updated = await _repository.UpdateAsync(entity);
        return _mapper.Map<AppraisalQuestionDto?>(updated);
    }

    public async Task<bool> DeleteAsync(int questionId)
    {
        return await _repository.DeleteAsync(questionId);
    }
}
