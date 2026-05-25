using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EPMS.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace EPMS.Infrastructure.Services;

public class OtpService : IOtpService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<OtpService> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public OtpService(IDistributedCache cache, ILogger<OtpService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<string> GenerateOtpAsync(string email)
    {
        var otp = GenerateCryptographicOtp();
        var cacheKey = GetOtpCacheKey(email);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await _cache.SetStringAsync(cacheKey, otp, options);
        _logger.LogDebug("OTP generated for email: {Email}", email);
        return otp;
    }

    public async Task<bool> VerifyOtpAsync(string email, string otp)
    {
        var cacheKey = GetOtpCacheKey(email);
        var storedOtp = await _cache.GetStringAsync(cacheKey);

        if (string.IsNullOrEmpty(storedOtp))
        {
            _logger.LogWarning("OTP not found for email: {Email}", email);
            return false;
        }

        var storedOtpBytes = Encoding.UTF8.GetBytes(storedOtp);
        var inputOtpBytes = Encoding.UTF8.GetBytes(otp);

        return CryptographicOperations.FixedTimeEquals(storedOtpBytes, inputOtpBytes);
    }

    public async Task InvalidateOtpAsync(string email)
    {
        var cacheKey = GetOtpCacheKey(email);
        await _cache.RemoveAsync(cacheKey);
        _logger.LogDebug("OTP invalidated for email: {Email}", email);
    }

    private static string GetOtpCacheKey(string email)
    {
        return $"otp:{email}";
    }

    private static string GenerateCryptographicOtp()
    {
        var otpBytes = new byte[4];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(otpBytes);
        var otpNumber = BitConverter.ToUInt32(otpBytes, 0) % 1000000;
        return otpNumber.ToString("D6");
    }
}
