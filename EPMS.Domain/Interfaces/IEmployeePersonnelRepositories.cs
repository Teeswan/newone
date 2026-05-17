using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface ILevelRepository : IBaseRepository<Level, string>
{
    Task<Level?> GetByIdNoTrackingAsync(string id);
}

public interface IPositionRepository : IBaseRepository<Position, int>
{
    Task<IEnumerable<Position>> GetPositionsByLevelAsync(string levelId);
    Task<Position?> GetByIdNoTrackingAsync(int id);
}
