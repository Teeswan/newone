using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EPMS.Application.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(Employee employee, User? user = null, CancellationToken cancellationToken = default);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly IPositionPermissionRepository _positionPermissionRepository;

    public TokenService(IConfiguration config, IPositionPermissionRepository positionPermissionRepository)
    {
        _config = config;
        _positionPermissionRepository = positionPermissionRepository;
    }

    public async Task<string> CreateTokenAsync(
        Employee employee,
        User? user = null,
        CancellationToken cancellationToken = default)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, employee.FullName ?? employee.Email ?? ""),
            new(ClaimTypes.Email, employee.Email ?? ""),
            new(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString()),
            new("EmployeeId", employee.EmployeeId.ToString()),
            new("name", employee.FullName ?? ""),
            new("role", user?.EmployeeId != null ? "User" : "Guest")
        };

        if (user != null)
        {
            claims.Add(new Claim("UserId", user.UserId.ToString()));
        }

        if (employee.PositionId != null)
        {
            claims.Add(new Claim("PositionId", employee.PositionId.Value.ToString()));

            var permissions = await _positionPermissionRepository.GetPermissionsByPositionAsync(employee.PositionId.Value);
            foreach (var permission in permissions)
            {
                if (!string.IsNullOrWhiteSpace(permission.PermissionCode))
                {
                    claims.Add(new Claim(AuthClaimTypes.Permission, permission.PermissionCode));
                }
            }
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiryInMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
