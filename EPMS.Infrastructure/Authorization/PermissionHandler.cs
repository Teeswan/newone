using Microsoft.AspNetCore.Authorization;
using EPMS.Domain.Interfaces;
using System.Security.Claims;

namespace EPMS.Infrastructure.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPositionPermissionRepository _repository;

    public PermissionHandler(IPositionPermissionRepository repository)
    {
        _repository = repository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // Extract PositionId from user claims
        var positionIdClaim = context.User.FindFirst("PositionId")?.Value;
        if (string.IsNullOrEmpty(positionIdClaim) || !int.TryParse(positionIdClaim, out int positionId))
        {
            return;
        }

        // Check if the position has the required permission
        var permissions = await _repository.GetPermissionsByPositionAsync(positionId);
        var userPermissions = permissions.Select(p => p.PermissionCode).ToList();

        if (userPermissions.Any(p => p == requirement.Permission))
        {
            context.Succeed(requirement);
            return;
        }

        // Handle "Manage" permission logic (CUD = Manage)
        // If the required permission is a Create, Edit, or Delete permission,
        // check if the user has the corresponding "Manage" permission.
        if (IsCudPermission(requirement.Permission))
        {
            var managePermission = GetManagePermission(requirement.Permission);
            if (userPermissions.Any(p => p == managePermission))
            {
                context.Succeed(requirement);
            }
        }
    }

    private bool IsCudPermission(string permission)
    {
        return permission.EndsWith(".Create") || 
               permission.EndsWith(".Edit") || 
               permission.EndsWith(".Delete") ||
               permission.EndsWith(".AssignPermissions") || // Special case for Security
               permission.EndsWith(".RevokePermissions");   // Special case for Security
    }

    private string GetManagePermission(string permission)
    {
        if (permission.Contains(".Security."))
        {
            return "Permissions.Security.Manage";
        }

        var parts = permission.Split('.');
        if (parts.Length >= 2)
        {
            // Reconstruct the category part (e.g., Permissions.Departments) and add .Manage
            return $"{parts[0]}.{parts[1]}.Manage";
        }

        return string.Empty;
    }
}
