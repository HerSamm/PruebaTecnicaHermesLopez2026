using PruebaTecnicaHermes.Api.Application.Addresses.Commands;
using PruebaTecnicaHermes.Api.Application.Addresses.Queries;

namespace PruebaTecnicaHermes.Api.Endpoints;

public static class AddressEndpoints
{
    public static void MapAddressEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/").WithTags("Addresses");

        group.MapPost("/users/{userId:int}/addresses",
                async (int userId, CreateAddressCommand command, CreateAddressHandler handler, CancellationToken ct) =>
                {
                    var address = await handler.HandleAsync(userId, command, ct);
                    return TypedResults.Created($"/addresses/{address.Id}", address);
                })
            .WithValidation<CreateAddressCommand>()
            .WithSummary("Crea una dirección para un usuario");

        group.MapGet("/users/{userId:int}/addresses",
                async (int userId, GetAddressesByUserHandler handler, CancellationToken ct) =>
                    TypedResults.Ok(await handler.HandleAsync(userId, ct)))
            .WithSummary("Lista las direcciones de un usuario");

        group.MapPut("/addresses/{id:int}",
                async (int id, UpdateAddressCommand command, UpdateAddressHandler handler, CancellationToken ct) =>
                    TypedResults.Ok(await handler.HandleAsync(id, command, ct)))
            .WithValidation<UpdateAddressCommand>()
            .WithSummary("Modifica una dirección");

        group.MapDelete("/addresses/{id:int}",
                async (int id, DeleteAddressHandler handler, CancellationToken ct) =>
                {
                    await handler.HandleAsync(id, ct);
                    return TypedResults.NoContent();
                })
            .WithSummary("Elimina una dirección");
    }
}
