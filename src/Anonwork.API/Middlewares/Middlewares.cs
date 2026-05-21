using Anonwork.Application.Common.Exceptions;
using Anonwork.Domain.Common.Exceptions;
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
            logger.LogError(ex, "Unhandled exception");
            await HandleAsync(ctx, ex);
        }
    }

    private static Task HandleAsync(HttpContext ctx, Exception ex)
    {
        var (status, message) = ex switch
        {
            ConflictException e => (HttpStatusCode.Conflict, e.Message),
            UnauthorizedException e => (HttpStatusCode.Unauthorized, e.Message),
            NotFoundException e => (HttpStatusCode.NotFound, e.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        ctx.Response.StatusCode = (int)status;
        ctx.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(new { error = message });
        return ctx.Response.WriteAsync(body);
    }
}