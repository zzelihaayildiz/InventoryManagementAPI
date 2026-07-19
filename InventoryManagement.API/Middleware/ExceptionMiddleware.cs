using System.Net;
using System.Text.Json;
using FluentValidation;
using InventoryManagement.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InventoryManagement.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }

        catch (NotFoundException ex)
        {
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