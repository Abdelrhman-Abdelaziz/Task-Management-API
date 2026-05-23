using System.Net;
using System.Text.Json;

using TaskManagement.Application.Common;
using TaskManagement.Application.Common.Exceptions;

namespace TaskManagement.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                ApiResponse<object>.Fail(ve.Message, ve.Errors)),

            UnauthorizedException ue => (
                HttpStatusCode.Unauthorized,
                ApiResponse<object>.Fail(ue.Message)),

            NotFoundException nf => (
                HttpStatusCode.NotFound,
                ApiResponse<object>.Fail(nf.Message)),

            ConflictException ce => (
                HttpStatusCode.Conflict,
                ApiResponse<object>.Fail(ce.Message)),

            _ => LogAndCreateInternalError(exception)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private (HttpStatusCode, ApiResponse<object>) LogAndCreateInternalError(Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");
        return (
            HttpStatusCode.InternalServerError,
            ApiResponse<object>.Fail("An unexpected error occurred."));
    }
}
