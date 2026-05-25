using MediatR;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using Microsoft.Extensions.Logging;

namespace EPMS.Application.UseCases.Auth.Commands;

public record ResetPasswordCommand(string Email, string Otp, string NewPassword) : IRequest<Result>;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IOtpService _otpService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IEmployeeRepository employeeRepository,
        IOtpService otpService,
        IPasswordHasher passwordHasher,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _otpService = otpService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeRepository.GetByEmailAsync(request.Email);
            if (employee == null)
            {
                _logger.LogWarning("Reset password attempted for non-existent email: {Email}", request.Email);
                return Result.Failure("Invalid OTP or email.");
            }

            var isOtpValid = await _otpService.VerifyOtpAsync(request.Email, request.Otp);
            if (!isOtpValid)
            {
                _logger.LogWarning("Invalid OTP for email: {Email}", request.Email);
                return Result.Failure("Invalid or expired OTP.");
            }

            employee.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            employee.IsFirstLogin = false;
            await _employeeRepository.UpdateAsync(employee);

            await _otpService.InvalidateOtpAsync(request.Email);

            _logger.LogInformation("Password reset successfully for email: {Email}", request.Email);
            return Result.Success("Password reset successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for email: {Email}", request.Email);
            return Result.Failure("An error occurred. Please try again later.");
        }
    }
}
