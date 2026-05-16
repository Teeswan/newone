using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class FormQuestionService : IFormQuestionService
{
    private readonly IFormQuestionRepository _repository;
    private readonly IMapper _mapper;

    public FormQuestionService(IFormQuestionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FormQuestionDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<FormQuestionDto>>(entities);
    }

    public async Task<IEnumerable<FormQuestionDto>> GetByFormIdAsync(int formId)
    {
        var entities = await _repository.GetByFormIdAsync(formId);
        return _mapper.Map<IEnumerable<FormQuestionDto>>(entities);
    }

    public async Task<FormQuestionDto?> GetByFormAndQuestionIdAsync(int formId, int questionId)
    {
        var entity = await _repository.GetByFormAndQuestionIdAsync(formId, questionId);
        return _mapper.Map<FormQuestionDto?>(entity);
    }

    public async Task<FormQuestionDto> CreateAsync(CreateFormQuestionRequest request)
    {
        var entity = _mapper.Map<FormQuestion>(request);
        var created = await _repository.CreateAsync(entity);
        return _mapper.Map<FormQuestionDto>(created);
    }

    public async Task<FormQuestionDto?> UpdateAsync(int formId, int questionId, UpdateFormQuestionRequest request)
    {
        var entity = _mapper.Map<FormQuestion>(request);
        entity.FormId = formId;
        entity.QuestionId = questionId;

        var updated = await _repository.UpdateAsync(entity);
        return _mapper.Map<FormQuestionDto?>(updated);
    }

    public async Task<bool> DeleteAsync(int formId, int questionId)
    {
        return await _repository.DeleteAsync(formId, questionId);
    }
}
