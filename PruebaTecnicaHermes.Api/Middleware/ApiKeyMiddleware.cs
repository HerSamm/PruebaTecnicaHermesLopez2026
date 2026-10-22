namespace PruebaTecnicaHermes.Api.Middleware;

public class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
{
    public const string HeaderName = "X-API-KEY";

    private readonly string _apiKey = configuration["ApiKey"]
        ?? throw new InvalidOperationException("Falta configurar 'ApiKey' en appsettings.json.");

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey) || providedKey != _apiKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = $"API Key inválida o ausente. Envíe el header {HeaderName}." });
            return;
        }

        await next(context);
    }
}
