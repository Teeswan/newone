using System.Net;
using System.Text.Json;
using EPMS.Shared.Responses;

namespace EPMS.Api.Middleware
{
    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var message = _env.IsDevelopment() 
                    ? $"[DEV] {ex.Message} | Inner: {ex.InnerException?.Message}" 
                    : "An unexpected error occurred. Please try again later.";

                var response = ApiResponse<object>.Fail(message);

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
            }
        }
    }
}