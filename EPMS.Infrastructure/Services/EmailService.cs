using System.Net;
using System.Net.Mail;
using EPMS.Application.Interfaces;
using EPMS.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPMS.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly SmtpSettings _smtpSettings;

    public EmailService(
        ILogger<EmailService> logger,
        IOptions<SmtpSettings> smtpSettings)
    {
        _logger = logger;
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendOtpEmailAsync(string toEmail, string otp)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_smtpSettings.Host))
            {
                _logger.LogWarning("SMTP Host not configured - falling back to mock email");
                LogMockEmail(toEmail, otp);
                return;
            }

            using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port);
            smtpClient.EnableSsl = _smtpSettings.EnableSsl;
            
            if (!string.IsNullOrWhiteSpace(_smtpSettings.Username) && !string.IsNullOrWhiteSpace(_smtpSettings.Password))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
            }

            var fromAddress = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName);
            var toAddress = new MailAddress(toEmail);

            using var message = new MailMessage(fromAddress, toAddress);
            message.Subject = "Your EPMS Password Reset OTP";
            message.Body = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Password Reset OTP</title>
</head>
<body style=""margin: 0; padding: 0; background-color: #f4f6f8; font-family: Arial, sans-serif;"">
    <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background-color: #f4f6f8;"">
        <tr>
            <td align=""center"">
                <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" style=""background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08); margin: 40px 0;"">
                    <!-- Header -->
                    <tr>
                        <td align=""center"" style=""background-color: #1e88e5; border-top-left-radius: 12px; border-top-right-radius: 12px; padding: 30px 20px;"">
                            <div style=""color: #ffffff; font-size: 28px; font-weight: bold; letter-spacing: 1px;"">
                                🔐 EPMS System
                            </div>
                        </td>
                    </tr>
                    <!-- Body -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"">
                                <tr>
                                    <td align=""center"" style=""color: #1f2937; font-size: 24px; font-weight: bold; margin-bottom: 20px;"">
                                        Password Reset Request
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""color: #4b5563; font-size: 16px; line-height: 1.6; margin-bottom: 30px;"">
                                        Hello,
                                        <br><br>
                                        You have requested to reset your password for the EPMS system.
                                        <br><br>
                                        Please use the following One-Time Password (OTP) to proceed:
                                    </td>
                                </tr>
                                <!-- OTP Box -->
                                <tr>
                                    <td align=""center"">
                                        <div style=""background-color: #e3f2fd; border: 2px solid #1e88e5; border-radius: 8px; padding: 25px 40px; margin: 20px 0;"">
                                            <div style=""color: #1e88e5; font-size: 36px; font-weight: bold; letter-spacing: 8px;"">
                                                {otp}
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <!-- Expiration Note -->
                                <tr>
                                    <td align=""center"" style=""color: #ef4444; font-size: 14px; font-weight: 600; margin-bottom: 30px;"">
                                        ⚠️ This OTP will expire in 5 minutes
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""color: #4b5563; font-size: 16px; line-height: 1.6;"">
                                        If you did not request this password reset, please ignore this email. Your account remains secure.
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f9fafb; border-bottom-left-radius: 12px; border-bottom-right-radius: 12px; padding: 25px 30px;"">
                            <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"">
                                <tr>
                                    <td align=""center"" style=""color: #6b7280; font-size: 14px; margin-bottom: 10px;"">
                                        🚫 Do not share this OTP with anyone
                                    </td>
                                </tr>
                                <tr>
                                    <td align=""center"" style=""color: #9ca3af; font-size: 12px;"">
                                        © 2026 EPMS System. All rights reserved.
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
";
            message.IsBodyHtml = true;

            await smtpClient.SendMailAsync(message);
            _logger.LogInformation("OTP email sent successfully to: {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OTP email to: {ToEmail}", toEmail);
            LogMockEmail(toEmail, otp);
        }
    }

    private void LogMockEmail(string toEmail, string otp)
    {
        _logger.LogInformation("========================================");
        _logger.LogInformation("SENDING OTP EMAIL (MOCK/FALLBACK)");
        _logger.LogInformation("To: {ToEmail}", toEmail);
        _logger.LogInformation("OTP: {Otp}", otp);
        _logger.LogInformation("========================================");
    }
}
