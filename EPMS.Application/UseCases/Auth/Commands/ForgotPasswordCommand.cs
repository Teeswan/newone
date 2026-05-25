using MediatR;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using Microsoft.Extensions.Logging;

namespace EPMS.Application.UseCases.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IEmployeeRepository employeeRepository,
        IOtpService otpService,
        IEmailService emailService,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _otpService = otpService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeRepository.GetByEmailAsync(request.Email);
            if (employee == null)
            {
                _logger.LogWarning("Forgot password attempted for non-existent email: {Email}", request.Email);
                return Result.Success("If the email exists, an OTP has been sent.");
            }

            var otp = await _otpService.GenerateOtpAsync(request.Email);
            await _emailService.SendOtpEmailAsync(request.Email, otp);

            _logger.LogInformation("OTP generated and sent for email: {Email}", request.Email);
            return Result.Success("If the email exists, an OTP has been sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing forgot password for email: {Email}", request.Email);
            return Result.Failure("An error occurred. Please try again later.");
        }
    }
}
