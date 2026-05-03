using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class PerformanceEvaluationService : IPerformanceEvaluationService
{
    private readonly IPerformanceEvaluationRepository _repository;
    private readonly IMapper _mapper;

    public PerformanceEvaluationService(IPerformanceEvaluationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PerformanceEvaluationDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<PerformanceEvaluationDto>>(entities);
    }

    public async Task<PerformanceEvaluationDto?> GetByIdAsync(int evalId)
    {
        var entity = await _repository.GetByIdAsync(evalId);
        return _mapper.Map<PerformanceEvaluationDto?>(entity);
    }

    public async Task<IEnumerable<PerformanceEvaluationDto>> GetByEmployeeIdAsync(int employeeId)
    {
        var entities = await _repository.GetByEmployeeIdAsync(employeeId);
        return _mapper.Map<IEnumerable<PerformanceEvaluationDto>>(entities);
    }

    public async Task<IEnumerable<PerformanceEvaluationDto>> GetByCycleIdAsync(int cycleId)
    {
        var entities = await _repository.GetByCycleIdAsync(cycleId);
        return _mapper.Map<IEnumerable<PerformanceEvaluationDto>>(entities);
    }

    public async Task<PerformanceEvaluationDto> CreateAsync(CreatePerformanceEvaluationRequest request)
    {
        var entity = _mapper.Map<PerformanceEvaluation>(request);
        var created = await _repository.CreateAsync(entity);
        return _mapper.Map<PerformanceEvaluationDto>(created);
    }

    public async Task<PerformanceEvaluationDto?> UpdateAsync(int evalId, UpdatePerformanceEvaluationRequest request)
    {
        var entity = _mapper.Map<PerformanceEvaluation>(request);
        entity.EvalId = evalId;

        var updated = await _repository.UpdateAsync(entity);
        return _mapper.Map<PerformanceEvaluationDto?>(updated);
    }

    public async Task<bool> DeleteAsync(int evalId)
    {
        return await _repository.DeleteAsync(evalId);
    }
}
