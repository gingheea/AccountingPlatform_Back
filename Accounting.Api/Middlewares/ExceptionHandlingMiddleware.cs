using System.Net;
using System.Text.Json;
using Accounting.Application.Common.Errors;
using FluentValidation;

namespace Accounting.Api.Middlewares;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error");

            await WriteResponse(context, HttpStatusCode.BadRequest, new
            {
                type = "validation_error",
                title = "Validation failed",
                status = 400,
                errors = ex.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                })
            });
        }
        catch (NotFoundException ex)
        {
            await WriteResponse(context, HttpStatusCode.NotFound, new
            {
                type = "not_found",
                title = ex.Message,
                status = 404
            });
        }
        catch (ForbiddenException ex)
        {
            await WriteResponse(context, HttpStatusCode.Forbidden, new
            {
                type = "forbidden",
                title = ex.Message,
                status = 403
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteResponse(context, HttpStatusCode.Unauthorized, new
            {
                type = "unauthorized",
                title = ex.Message,
                status = 401
            });
        }
        catch (ConflictException ex)
        {
            await WriteResponse(context, HttpStatusCode.Conflict, new
            {
                type = "conflict",
                title = ex.Message,
                status = 409
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await WriteResponse(context, HttpStatusCode.InternalServerError, new
            {
                type = "server_error",
                title = "Unexpected error",
                status = 500
            });
        }
    }

    private static async Task WriteResponse(
        HttpContext context,
        HttpStatusCode statusCode,
        object payload)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
