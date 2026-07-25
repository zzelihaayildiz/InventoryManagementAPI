using System.Net;
using System.Text.Json;
using FluentValidation;
using InventoryManagement.Application.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger)
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

        catch (NotFoundException ex)
        {
            _logger.LogInformation(
                "Resource not found. Path: {Path}",
                context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = ex.Message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }

        catch (ValidationException ex)
        {
            _logger.LogWarning(
                ex,
                "Validation error on {Path}",
                context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = "Validation Hatası",
                errors = ex.Errors.Select(x => x.ErrorMessage)
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }

        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "UnHandled exception. Path:{Path} Method:{Method}",
                context.Request.Path,
                context.Request.Method);

            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = ex.Message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}