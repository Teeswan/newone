using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services;

public interface ILevelBlazorService
{
    Task<IEnumerable<LevelDto>> GetAllLevelsAsync();
    Task<LevelDto?> GetLevelByIdAsync(string id);
    Task<LevelDto> CreateLevelAsync(CreateLevelRequest request);
    Task<LevelDto?> UpdateLevelAsync(string id, UpdateLevelRequest request);
    Task<bool> DeleteLevelAsync(string id);
}
