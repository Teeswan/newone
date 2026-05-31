using Blazored.LocalStorage;
using EPMS.Shared.Constants;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace EPMS.Blazor.Services;

public class PermissionCheckerService : IPermissionCheckerService
{
    private readonly IPermissionBlazorService _permissionBlazorService;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    
    private List<string>? _cachedPermissions;
    private int? _cachedPositionId;
    
    public PermissionCheckerService(
        IPermissionBlazorService permissionBlazorService,
        AuthenticationStateProvider authStateProvider,
        ILocalStorageService localStorage)
    {
        _permissionBlazorService = permissionBlazorService;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
        
        _authStateProvider.AuthenticationStateChanged += OnAuthStateChanged;
    }

    private async void OnAuthStateChanged(Task<AuthenticationState> authStateTask)
    {
        await ClearCacheAsync();
    }

    private async Task<List<string>> GetCurrentUserPermissionsAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        
        var positionIdClaim = user.FindFirst("PositionId")?.Value;
        
        if (string.IsNullOrEmpty(positionIdClaim) || !int.TryParse(positionIdClaim, out int positionId))
        {
            return new List<string>();
        }
        
        if (_cachedPositionId == positionId && _cachedPermissions != null)
        {
            return _cachedPermissions;
        }
        
        var permissions = await _permissionBlazorService.GetPermissionsByPositionAsync(positionId);
        var permissionCodes = permissions.Select(p => p.PermissionCode ?? "").ToList();
        
        _cachedPositionId = positionId;
        _cachedPermissions = permissionCodes;
        
        return permissionCodes;
    }

    public async Task<bool> HasPermissionAsync(string permissionCode)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        if (HasPermissionClaim(authState.User, permissionCode))
        {
            return true;
        }

        var permissions = await GetCurrentUserPermissionsAsync();
        
        if (permissions.Contains(permissionCode))
        {
            return true;
        }

        var managePermission = GetManagePermission(permissionCode);
        if (!string.IsNullOrEmpty(managePermission) && permissions.Contains(managePermission))
        {
            return true;
        }
        
        return false;
    }

    private static bool HasPermissionClaim(ClaimsPrincipal user, string permissionCode)
    {
        if (user.HasClaim(AuthClaimTypes.Permission, permissionCode))
        {
            return true;
        }

        var managePermission = GetManagePermissionStatic(permissionCode);
        return !string.IsNullOrEmpty(managePermission)
            && user.HasClaim(AuthClaimTypes.Permission, managePermission);
    }

    private static string GetManagePermissionStatic(string permission)
    {
        if (permission.Contains(".Security."))
        {
            return Permissions.Security.Manage;
        }

        var parts = permission.Split('.');
        if (parts.Length >= 2)
        {
            return $"{parts[0]}.{parts[1]}.Manage";
        }

        return string.Empty;
    }

    public async Task<bool> HasViewPermissionAsync(string category)
    {
        return await HasPermissionAsync($"Permissions.{category}.View");
    }

    public async Task<bool> HasManagePermissionAsync(string category)
    {
        return await HasPermissionAsync($"Permissions.{category}.Manage");
    }

    public Task<bool> HasTeamEmployeeManagementAsync()
        => HasPermissionAsync(Permissions.Employees.TeamEmployeeManagement);

    public Task<bool> HasDepartmentScopedManagementAsync()
        => HasPermissionAsync(Permissions.DepartmentScopedManagement);

    public async Task<bool> CanCreateAsync(string category)
    {
        return await HasPermissionAsync($"Permissions.{category}.Create") || 
               await HasManagePermissionAsync(category);
    }

    public async Task<bool> CanEditAsync(string category)
    {
        return await HasPermissionAsync($"Permissions.{category}.Edit") || 
               await HasManagePermissionAsync(category);
    }

    public async Task<bool> CanDeleteAsync(string category)
    {
        return await HasPermissionAsync($"Permissions.{category}.Delete") || 
               await HasManagePermissionAsync(category);
    }

    public async Task ClearCacheAsync()
    {
        _cachedPermissions = null;
        _cachedPositionId = null;
    }

    private bool IsCudPermission(string permission)
    {
        return permission.EndsWith(".Create") || 
               permission.EndsWith(".Edit") || 
               permission.EndsWith(".Delete") ||
               permission.EndsWith(".AssignPermissions") ||
               permission.EndsWith(".RevokePermissions");
    }

    private string GetManagePermission(string permission)
    {
        return GetManagePermissionStatic(permission);
    }
}
