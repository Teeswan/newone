using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class PerformanceOutcomeService : IPerformanceOutcomeService
{
    private readonly IPerformanceOutcomeRepository _repository;

    public PerformanceOutcomeService(IPerformanceOutcomeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PerformanceOutcomeDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MapToDto);
    }

    public async Task<PerformanceOutcomeDto?> GetByIdAsync(int outcomeId)
    {
        var entity = await _repository.GetByIdAsync(outcomeId);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<IEnumerable<PerformanceOutcomeDto>> GetByEmployeeIdAsync(int employeeId)
    {
        var entities = await _repository.GetByEmployeeIdAsync(employeeId);
        return entities.Select(MapToDto);
    }

    public async Task<IEnumerable<PerformanceOutcomeDto>> GetByCycleIdAsync(int cycleId)
    {
        var entities = await _repository.GetByCycleIdAsync(cycleId);
        return entities.Select(MapToDto);
    }

    public async Task<PerformanceOutcomeDto> CreateAsync(CreatePerformanceOutcomeRequest request)
    {
        var entity = new PerformanceOutcome
        {
            EvalId = request.EvalId,
            EmployeeId = request.EmployeeId,
            CycleId = request.CycleId,
            RecommendationType = request.RecommendationType,
            OldPositionId = request.OldPositionId,
            NewPositionId = request.NewPositionId,
            OldLevelId = request.OldLevelId,
            NewLevelId = request.NewLevelId,
            ApprovalStatus = request.ApprovalStatus,
            EffectiveDate = request.EffectiveDate
        };

        var created = await _repository.CreateAsync(entity);
        return MapToDto(created);
    }

    public async Task<PerformanceOutcomeDto?> UpdateAsync(int outcomeId, UpdatePerformanceOutcomeRequest request)
    {
        var entity = new PerformanceOutcome
        {
            OutcomeId = outcomeId,
            EvalId = request.EvalId,
            EmployeeId = request.EmployeeId,
            CycleId = request.CycleId,
            RecommendationType = request.RecommendationType,
            OldPositionId = request.OldPositionId,
            NewPositionId = request.NewPositionId,
            OldLevelId = request.OldLevelId,
            NewLevelId = request.NewLevelId,
            ApprovalStatus = request.ApprovalStatus,
            EffectiveDate = request.EffectiveDate
        };

        var updated = await _repository.UpdateAsync(entity);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int outcomeId)
    {
        return await _repository.DeleteAsync(outcomeId);
    }

    private static PerformanceOutcomeDto MapToDto(PerformanceOutcome entity)
    {
        return new PerformanceOutcomeDto
        {
            OutcomeId = entity.OutcomeId,
            EvalId = entity.EvalId,
            EmployeeId = entity.EmployeeId,
            CycleId = entity.CycleId,
            RecommendationType = entity.RecommendationType,
            OldPositionId = entity.OldPositionId,
            NewPositionId = entity.NewPositionId,
            OldLevelId = entity.OldLevelId,
            NewLevelId = entity.NewLevelId,
            ApprovalStatus = entity.ApprovalStatus,
            EffectiveDate = entity.EffectiveDate
        };
    }
}
