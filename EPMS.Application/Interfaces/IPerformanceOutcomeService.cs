using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

public interface IPerformanceOutcomeService
{
    Task<IEnumerable<PerformanceOutcomeDto>> GetAllAsync();
    Task<PerformanceOutcomeDto?> GetByIdAsync(int outcomeId);
    Task<IEnumerable<PerformanceOutcomeDto>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<PerformanceOutcomeDto>> GetByCycleIdAsync(int cycleId);
    Task<PerformanceOutcomeDto> CreateAsync(CreatePerformanceOutcomeRequest request);
    Task<PerformanceOutcomeDto?> UpdateAsync(int outcomeId, UpdatePerformanceOutcomeRequest request);
    Task<bool> DeleteAsync(int outcomeId);
}
