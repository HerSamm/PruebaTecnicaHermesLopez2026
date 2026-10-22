using PruebaTecnicaHermes.Api.Application.Currencies.Commands;
using PruebaTecnicaHermes.Api.Application.Currencies.Queries;
using PruebaTecnicaHermes.Api.Application.CurrencyConversion;

namespace PruebaTecnicaHermes.Api.Endpoints;

public static class CurrencyEndpoints
{
    public static void MapCurrencyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/").WithTags("Currencies");

        group.MapGet("/currencies", async (GetCurrenciesHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(ct)))
            .WithSummary("Lista las monedas");

        group.MapPost("/currencies", async (CreateCurrencyCommand command, CreateCurrencyHandler handler, CancellationToken ct) =>
            {
                var currency = await handler.HandleAsync(command, ct);
                return TypedResults.Created($"/currencies/{currency.Id}", currency);
            })
            .WithValidation<CreateCurrencyCommand>()
            .WithSummary("Crea una moneda");

        group.MapPost("/currency/convert", async (ConvertCurrencyCommand command, ConvertCurrencyHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(command, ct)))
            .WithValidation<ConvertCurrencyCommand>()
            .WithSummary("Convierte un monto entre dos monedas");
    }
}
