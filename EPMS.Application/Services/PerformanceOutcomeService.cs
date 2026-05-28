using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class PerformanceOutcomeService : IPerformanceOutcomeService
{
    private readonly IPerformanceOutcomeRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public PerformanceOutcomeService(IPerformanceOutcomeRepository repository, IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PerformanceOutcomeDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<PerformanceOutcomeDto>>(entities);
    }

    public async Task<PerformanceOutcomeDto?> GetByIdAsync(int outcomeId)
    {
        var entity = await _repository.GetByIdAsync(outcomeId);
        return _mapper.Map<PerformanceOutcomeDto?>(entity);
    }

    public async Task<IEnumerable<PerformanceOutcomeDto>> GetByEmployeeIdAsync(int employeeId)
    {
        var entities = await _repository.GetByEmployeeIdAsync(employeeId);
        return _mapper.Map<IEnumerable<PerformanceOutcomeDto>>(entities);
    }

    public async Task<IEnumerable<PerformanceOutcomeDto>> GetByCycleIdAsync(int cycleId)
    {
        var entities = await _repository.GetByCycleIdAsync(cycleId);
        return _mapper.Map<IEnumerable<PerformanceOutcomeDto>>(entities);
    }

    public async Task<PerformanceOutcomeDto> CreateAsync(CreatePerformanceOutcomeRequest request)
    {
        var entity = _mapper.Map<PerformanceOutcome>(request);
        var created = await _repository.CreateAsync(entity);
        return _mapper.Map<PerformanceOutcomeDto>(created);
    }

    public async Task<PerformanceOutcomeDto?> UpdateAsync(int outcomeId, UpdatePerformanceOutcomeRequest request)
    {
        var existing = await _repository.GetByIdAsync(outcomeId);
        if (existing == null) return null;

        _mapper.Map(request, existing);
        existing.OutcomeId = outcomeId;

        var updated = await _repository.UpdateAsync(existing);

        // Automated Profile Update Logic
        if (updated != null && 
            updated.ApprovalStatus == "Approved" && 
            updated.EmployeeId.HasValue &&
            updated.EffectiveDate <= DateOnly.FromDateTime(DateTime.Now))
        {
            var employee = await _employeeRepository.GetByIdAsync(updated.EmployeeId.Value);
            if (employee != null)
            {
                bool changed = false;
                if (updated.NewPositionId.HasValue && employee.PositionId != updated.NewPositionId)
                {
                    employee.PositionId = updated.NewPositionId.Value;
                    changed = true;
                }

                if (changed)
                {
                    await _employeeRepository.UpdateAsync(employee);
                }
            }
        }

        return _mapper.Map<PerformanceOutcomeDto?>(updated);
    }

    public async Task<bool> DeleteAsync(int outcomeId)
    {
        return await _repository.DeleteAsync(outcomeId);
    }
}
