using Anonwork.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Anonwork.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            if (IsClientError(ex))
                logger.LogWarning(ex, "Client error occurred");
            else
                logger.LogError(ex, "Unhandled exception");

            await HandleAsync(ctx, ex);
        }
    }

    private static bool IsClientError(Exception ex) => ex is BadRequestException or ConflictException or UnauthorizedException or NotFoundException;

    private static Task HandleAsync(HttpContext ctx, Exception ex)
    {
        var (status, code, message) = ex switch
        {
            BadRequestException e => (HttpStatusCode.BadRequest, "bad_request", e.Message),
            ConflictException e => (HttpStatusCode.Conflict, "conflict", e.Message),
            UnauthorizedException e => (HttpStatusCode.Unauthorized, "unauthorized", e.Message),
            NotFoundException e => (HttpStatusCode.NotFound, "not_found", e.Message),
            _ => (HttpStatusCode.InternalServerError, "internal_server_error", "An unexpected error occurred.")
        };

        ctx.Response.StatusCode = (int)status;
        ctx.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(new
        {
            status = (int)status,
            error = new
            {
                code,
                message
            }
        });

        return ctx.Response.WriteAsync(body);
    }
}