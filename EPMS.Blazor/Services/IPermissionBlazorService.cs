using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Blazor.Services;

public interface IPermissionBlazorService
{
    Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
    Task<IEnumerable<PermissionDto>> GetPermissionsByPositionAsync(int positionId);
    Task<bool> AssignPermissionAsync(AssignPermissionRequest request);
    Task<bool> RevokePermissionAsync(RevokePermissionRequest request);
}
