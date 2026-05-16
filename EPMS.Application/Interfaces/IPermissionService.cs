using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<PermissionDto>> GetAllAsync();
    Task<IEnumerable<PermissionDto>> GetByPositionAsync(int positionId);
    Task<bool> AssignPermissionAsync(AssignPermissionRequest request);
    Task<bool> RevokePermissionAsync(RevokePermissionRequest request);
}
