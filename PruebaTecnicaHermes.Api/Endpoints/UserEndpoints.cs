using PruebaTecnicaHermes.Api.Application.Users.Commands;
using PruebaTecnicaHermes.Api.Application.Users.Queries;

namespace PruebaTecnicaHermes.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Users");

        group.MapGet("/", async (bool? isActive, GetUsersHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(new GetUsersQuery(isActive), ct)))
            .WithSummary("Lista los usuarios. Filtro opcional: ?isActive=true|false");

        group.MapGet("/{id:int}", async (int id, GetUserByIdHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(id, ct)))
            .WithSummary("Obtiene un usuario por id");

        group.MapPost("/", async (CreateUserCommand command, CreateUserHandler handler, CancellationToken ct) =>
            {
                var user = await handler.HandleAsync(command, ct);
                return TypedResults.Created($"/users/{user.Id}", user);
            })
            .WithValidation<CreateUserCommand>()
            .WithSummary("Crea un usuario");

        group.MapPut("/{id:int}", async (int id, UpdateUserCommand command, UpdateUserHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(id, command, ct)))
            .WithValidation<UpdateUserCommand>()
            .WithSummary("Modifica name, email e isActive de un usuario");

        group.MapDelete("/{id:int}", async (int id, DeleteUserHandler handler, CancellationToken ct) =>
            {
                await handler.HandleAsync(id, ct);
                return TypedResults.NoContent();
            })
            .WithSummary("Elimina un usuario (y sus direcciones)");

        group.MapPost("/bulk", async (BulkCreateUsersCommand command, BulkCreateUsersHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(command, ct)))
            .WithValidation<BulkCreateUsersCommand>()
            .WithSummary("Carga masiva de usuarios, procesados en paralelo");
    }
}
