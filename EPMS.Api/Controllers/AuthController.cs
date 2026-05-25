using EPMS.Application.Interfaces;
using EPMS.Application.Services;
using EPMS.Application.UseCases.Auth.Commands;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EPMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;
    private readonly IAuthService _authService;
    private readonly IMediator _mediator;
    private readonly IAccountInitializationService _accountInitializationService;

    public AuthController(
        IEmployeeRepository employeeRepository, 
        ITokenService tokenService, 
        IConfiguration config,
        IAuthService authService,
        IMediator mediator,
        IAccountInitializationService accountInitializationService)
    {
        _employeeRepository = employeeRepository;
        _tokenService = tokenService;
        _config = config;
        _authService = authService;
        _mediator = mediator;
        _accountInitializationService = accountInitializationService;
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null)
        {
            return Unauthorized("Invalid email or password.");
        }
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("bulk-create-accounts")]
    public async Task<IActionResult> BulkCreateAccounts()
    {
        var result = await _mediator.Send(new BulkCreateAccountsCommand());
        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var command = new ChangePasswordCommand 
        { 
            EmployeeId = request.EmployeeId, 
            NewPassword = request.NewPassword 
        };
        var result = await _mediator.Send(command);
        if (result)
            return Ok();
        return BadRequest("Failed to change password");
    }
    
    [Authorize]
    [HttpPost("initialize-employee/{employeeId}")]
    public async Task<IActionResult> InitializeEmployee(int employeeId, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
            return NotFound("Employee not found");
            
        await _accountInitializationService.InitializeAccountAsync(employee, cancellationToken);
        return Ok("Employee initialized successfully");
    }

    [HttpGet("test-token/{positionId}")]
    public IActionResult GetTestToken(int positionId)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim("PositionId", positionId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { Token = tokenString });
    }

    [AllowAnonymous]
    [HttpPost("update-system-settings")]
    public async Task<IActionResult> UpdateSystemSettings([FromBody] UpdateSystemSettingsRequest request)
    {
        var command = new UpdateSystemSettingsCommand 
        { 
            NewDefaultPassword = request.NewDefaultPassword 
        };
        var result = await _mediator.Send(command);
        if (result)
            return Ok();
        return BadRequest("Failed to update system settings");
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var command = new ForgotPasswordCommand(request.Email);
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var command = new ResetPasswordCommand(request.Email, request.Otp, request.NewPassword);
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }
}
