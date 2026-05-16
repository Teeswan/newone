using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using EPMS.Application.Exceptions;
using Serilog;

namespace EPMS.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled Exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        if (exception is MeetingConflictException conflictException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var conflictResult = JsonSerializer.Serialize(new
            {
                error = conflictException.Message,
                conflicts = conflictException.ConflictingMeetings
            });
            return context.Response.WriteAsync(conflictResult);
        }

        context.Response.StatusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = exception.Message }));
    }
}