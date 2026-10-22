using PruebaTecnicaHermes.Api.Application.Common;

namespace PruebaTecnicaHermes.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (BadHttpRequestException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error no controlado");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado.");
        }
    }

    private static Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new { error = message });
    }
}
