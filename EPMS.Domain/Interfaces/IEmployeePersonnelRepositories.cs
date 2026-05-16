using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface ILevelRepository : IBaseRepository<Level, string>
{
}

public interface IPositionRepository : IBaseRepository<Position, int>
{
    Task<IEnumerable<Position>> GetPositionsByLevelAsync(string levelId);
}
