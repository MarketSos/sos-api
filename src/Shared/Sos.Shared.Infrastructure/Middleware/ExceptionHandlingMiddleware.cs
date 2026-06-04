using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sos.Shared.Kernel.Exceptions;
using System.Text.Json;
using ValidationException = Sos.Shared.Kernel.Exceptions.ValidationException;

namespace Sos.Shared.Infrastructure.Middleware;

/// <summary>
/// Global exception handler — barcha kutilmagan xatolarni ushlab ProblemDetails formatida qaytaradi.
///
/// Xaritalash:
///   ValidationException → 400 (validatsiya xatolari ro'yxati bilan)
///   NotFoundException   → 404
///   DomainException     → 422
///   boshqa Exception    → 500 (detallar logga yoziladi, foydalanuvchiga yashiriladi)
///
/// Middleware qo'shish: app.UseSosExceptionHandling()
/// </summary>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException ex)
        {
            await WriteAsync(ctx, StatusCodes.Status400BadRequest, new ProblemDetails
            {
                Title  = "Validatsiya xatosi",
                Status = 400,
                Type   = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Extensions =
                {
                    ["errors"] = ex.Errors
                        .GroupBy(e => e.Field)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.Message).ToArray())
                }
            });
        }
        catch (NotFoundException ex)
        {
            await WriteAsync(ctx, StatusCodes.Status404NotFound, new ProblemDetails
            {
                Title   = "Topilmadi",
                Detail  = ex.Message,
                Status  = 404,
                Type    = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            });
        }
        catch (DomainException ex)
        {
            await WriteAsync(ctx, StatusCodes.Status422UnprocessableEntity, new ProblemDetails
            {
                Title   = "Domain qoidasi buzildi",
                Detail  = ex.Message,
                Status  = 422,
                Type    = "https://tools.ietf.org/html/rfc4918#section-11.2"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kutilmagan xato: {Method} {Path}", ctx.Request.Method, ctx.Request.Path);

            await WriteAsync(ctx, StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title  = "Server xatosi",
                Detail = "Ichki server xatosi yuz berdi. Iltimos keyinroq urinib ko'ring.",
                Status = 500,
                Type   = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            });
        }
    }

    private static async Task WriteAsync(HttpContext ctx, int statusCode, ProblemDetails problem)
    {
        ctx.Response.StatusCode  = statusCode;
        ctx.Response.ContentType = "application/problem+json";
        problem.Instance = ctx.Request.Path;

        await ctx.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOpts));
    }
}
