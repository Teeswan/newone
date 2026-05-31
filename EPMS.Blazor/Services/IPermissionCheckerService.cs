namespace EPMS.Blazor.Services;

public interface IPermissionCheckerService
{
    Task<bool> HasPermissionAsync(string permissionCode);
    Task<bool> HasViewPermissionAsync(string category);
    Task<bool> HasManagePermissionAsync(string category);
    Task<bool> HasTeamEmployeeManagementAsync();
    Task<bool> CanCreateAsync(string category);
    Task<bool> CanEditAsync(string category);
    Task<bool> CanDeleteAsync(string category);
    Task ClearCacheAsync();
}
