using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class PerformanceEvaluationService : IPerformanceEvaluationService
{
    private readonly IPerformanceEvaluationRepository _repository;

    public PerformanceEvaluationService(IPerformanceEvaluationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PerformanceEvaluationDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MapToDto);
    }

    public async Task<PerformanceEvaluationDto?> GetByIdAsync(int evalId)
    {
        var entity = await _repository.GetByIdAsync(evalId);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<IEnumerable<PerformanceEvaluationDto>> GetByEmployeeIdAsync(int employeeId)
    {
        var entities = await _repository.GetByEmployeeIdAsync(employeeId);
        return entities.Select(MapToDto);
    }

    public async Task<IEnumerable<PerformanceEvaluationDto>> GetByCycleIdAsync(int cycleId)
    {
        var entities = await _repository.GetByCycleIdAsync(cycleId);
        return entities.Select(MapToDto);
    }

    public async Task<PerformanceEvaluationDto> CreateAsync(CreatePerformanceEvaluationRequest request)
    {
        var entity = new PerformanceEvaluation
        {
            EmployeeId = request.EmployeeId,
            CycleId = request.CycleId,
            FinalRatingScore = request.FinalRatingScore,
            IsFinalized = request.IsFinalized,
            FinalizedAt = request.FinalizedAt
        };

        var created = await _repository.CreateAsync(entity);
        return MapToDto(created);
    }

    public async Task<PerformanceEvaluationDto?> UpdateAsync(int evalId, UpdatePerformanceEvaluationRequest request)
    {
        var entity = new PerformanceEvaluation
        {
            EvalId = evalId,
            EmployeeId = request.EmployeeId,
            CycleId = request.CycleId,
            FinalRatingScore = request.FinalRatingScore,
            IsFinalized = request.IsFinalized,
            FinalizedAt = request.FinalizedAt
        };

        var updated = await _repository.UpdateAsync(entity);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int evalId)
    {
        return await _repository.DeleteAsync(evalId);
    }

    private static PerformanceEvaluationDto MapToDto(PerformanceEvaluation entity)
    {
        return new PerformanceEvaluationDto
        {
            EvalId = entity.EvalId,
            EmployeeId = entity.EmployeeId,
            CycleId = entity.CycleId,
            FinalRatingScore = entity.FinalRatingScore,
            IsFinalized = entity.IsFinalized,
            FinalizedAt = entity.FinalizedAt
        };
    }
}
