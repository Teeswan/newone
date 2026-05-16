using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services
{
    public interface IPerformanceOutcomeBlazorService
    {
        Task<IEnumerable<PerformanceOutcomeDto>> GetAllPerformanceOutcomesAsync();
        Task<PerformanceOutcomeDto?> GetPerformanceOutcomeByIdAsync(int id);
        Task<PerformanceOutcomeDto> CreatePerformanceOutcomeAsync(CreatePerformanceOutcomeRequest request);
        Task<PerformanceOutcomeDto?> UpdatePerformanceOutcomeAsync(int id, UpdatePerformanceOutcomeRequest request);
        Task<bool> DeletePerformanceOutcomeAsync(int id);
    }
}
