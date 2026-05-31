using EPMS.Shared.Constants;
using Microsoft.AspNetCore.Authorization;

namespace EPMS.Blazor.Authorization;

public static class BlazorAuthorizationExtensions
{
    /// <summary>
    /// Registers Blazor authorization policies that map to JWT permission claims.
    /// </summary>
    public static IServiceCollection AddEpmsAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy(Permissions.DepartmentScopedManagement, policy =>
                policy.RequireClaim(AuthClaimTypes.Permission, Permissions.DepartmentScopedManagement));

            options.AddPolicy(Permissions.Employees.TeamEmployeeManagement, policy =>
                policy.RequireClaim(AuthClaimTypes.Permission, Permissions.Employees.TeamEmployeeManagement));
        });

        return services;
    }
}
