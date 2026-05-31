using System.Net;
using System.Text.Json;
using EPMS.Shared.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Api.Middleware
{
    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
                
                context.Response.ContentType = "application/json";
                var statusCode = HttpStatusCode.InternalServerError;
                var message = "An unexpected error occurred. Please try again later.";

                // Handle specific database exceptions
                if (ex is DbUpdateException dbEx && dbEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Unique constraint / Primary key violation
                    {
                        statusCode = HttpStatusCode.Conflict;
                        message = "This record already exists. Please check for duplicate IDs or names.";
                    }
                }
                else if (ex is InvalidOperationException)
                {
                    statusCode = HttpStatusCode.BadRequest;
                    message = ex.Message;
                }
                else if (ex is UnauthorizedAccessException)
                {
                    statusCode = HttpStatusCode.Forbidden;
                    message = ex.Message;
                }

                context.Response.StatusCode = (int)statusCode;

                var response = ApiResponse<object>.FailureResponse(message);

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
            }
        }
    }
}