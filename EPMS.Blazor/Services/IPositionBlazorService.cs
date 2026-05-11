using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services;

public interface IPositionBlazorService
{
    Task<IEnumerable<PositionDto>> GetAllPositionsAsync();
    Task<PositionDto?> GetPositionByIdAsync(int id);
    Task<PositionDto> CreatePositionAsync(CreatePositionRequest request);
    Task<PositionDto?> UpdatePositionAsync(int id, UpdatePositionRequest request);
    Task<bool> DeletePositionAsync(int id);
    Task<IEnumerable<PositionDto>> GetPositionsByLevelAsync(string levelId);
}
