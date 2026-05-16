using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IPositionPermissionRepository
{
    Task<IEnumerable<Permission>> GetPermissionsByPositionAsync(int positionId);
    Task<PositionPermission?> GetByPositionAndPermissionAsync(int positionId, int permissionId);
    Task<PositionPermission> CreateAsync(PositionPermission entity);
    Task<bool> DeleteAsync(int positionId, int permissionId);
}
