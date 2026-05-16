using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPMS.Application.Services;

public class AppraisalResponseService : IAppraisalResponseService
{
    private readonly IAppraisalResponseRepository _repository;
    private readonly IPerformanceEvaluationRepository _evaluationRepository;
    private readonly IMapper _mapper;

    public AppraisalResponseService(
        IAppraisalResponseRepository repository, 
        IPerformanceEvaluationRepository evaluationRepository,
        IMapper mapper)
    {
        _repository = repository;
        _evaluationRepository = evaluationRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AppraisalResponseDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<AppraisalResponseDto>>(entities);
    }

    public async Task<AppraisalResponseDto?> GetByIdAsync(long responseId)
    {
        var entity = await _repository.GetByIdAsync(responseId);
        return _mapper.Map<AppraisalResponseDto?>(entity);
    }

    public async Task<IEnumerable<AppraisalResponseDto>> GetByEvalIdAsync(int evalId, int? currentEmployeeId = null)
    {
        var entities = await _repository.GetByEvalIdAsync(evalId);
        var evaluation = await _evaluationRepository.GetByIdAsync(evalId);

        if (evaluation != null && currentEmployeeId.HasValue)
        {
            // If the viewer is NOT the employee being reviewed
            if (evaluation.EmployeeId != currentEmployeeId)
            {
                // If status is not yet SelfSubmitted, hide 'Self' responses
                if (evaluation.Status < PerformanceEvaluationStatus.SelfSubmitted)
                {
                    entities = entities.Where(r => r.RespondentRole != "Self");
                }
            }
        }

        return _mapper.Map<IEnumerable<AppraisalResponseDto>>(entities);
    }

    public async Task<AppraisalResponseDto> CreateAsync(CreateAppraisalResponseRequest request)
    {
        var entity = _mapper.Map<AppraisalResponse>(request);
        var created = await _repository.CreateAsync(entity);
        return _mapper.Map<AppraisalResponseDto>(created);
    }

    public async Task<AppraisalResponseDto?> UpdateAsync(long responseId, UpdateAppraisalResponseRequest request)
    {
        var entity = _mapper.Map<AppraisalResponse>(request);
        entity.ResponseId = responseId;

        var updated = await _repository.UpdateAsync(entity);
        return _mapper.Map<AppraisalResponseDto?>(updated);
    }

    public async Task<bool> DeleteAsync(long responseId)
    {
        return await _repository.DeleteAsync(responseId);
    }
}
