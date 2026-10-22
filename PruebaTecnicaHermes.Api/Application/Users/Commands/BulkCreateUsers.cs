using FluentValidation;
using PruebaTecnicaHermes.Api.Application.Common;

namespace PruebaTecnicaHermes.Api.Application.Users.Commands;

public record BulkCreateUsersCommand(List<CreateUserCommand> Users);

public record BulkUserError(int Index, string? Email, string Reason);

public record BulkCreateUsersResult(
    int Total,
    int Created,
    int Failed,
    List<UserDto> CreatedUsers,
    List<BulkUserError> Errors);

public class BulkCreateUsersValidator : AbstractValidator<BulkCreateUsersCommand>
{
    public const int MaxUsersPerRequest = 1000;

    public BulkCreateUsersValidator()
    {
        RuleFor(x => x.Users)
            .NotEmpty().WithMessage("Debe enviar al menos un usuario.")
            .Must(users => users is null || users.Count <= MaxUsersPerRequest)
            .WithMessage($"No se pueden enviar más de {MaxUsersPerRequest} usuarios por petición.");
    }
}

public class BulkCreateUsersHandler(CreateUserHandler createUserHandler, IValidator<CreateUserCommand> userValidator)
{
    public async Task<BulkCreateUsersResult> HandleAsync(BulkCreateUsersCommand command, CancellationToken ct)
    {
        // Se lanzan todas las inserciones a la vez y se espera a que terminen con Task.WhenAll.
        // Es seguro porque CreateUserHandler crea un DbContext independiente (IDbContextFactory) por cada usuario.
        var tasks = command.Users.Select((user, index) => ProcessUserAsync(user, index, ct));
        var results = await Task.WhenAll(tasks);

        var createdUsers = results.Where(r => r.User is not null).Select(r => r.User!).ToList();
        var errors = results.Where(r => r.Error is not null).Select(r => r.Error!).ToList();

        return new BulkCreateUsersResult(
            Total: command.Users.Count,
            Created: createdUsers.Count,
            Failed: errors.Count,
            CreatedUsers: createdUsers,
            Errors: errors);
    }

    private async Task<BulkItemResult> ProcessUserAsync(CreateUserCommand? user, int index, CancellationToken ct)
    {
        if (user is null)
            return BulkItemResult.Fail(index, null, "El elemento está vacío.");

        var validation = await userValidator.ValidateAsync(user, ct);
        if (!validation.IsValid)
        {
            var reason = string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));
            return BulkItemResult.Fail(index, user.Email, reason);
        }

        try
        {
            var created = await createUserHandler.HandleAsync(user, ct);
            return BulkItemResult.Ok(created);
        }
        catch (ConflictException ex)
        {
            return BulkItemResult.Fail(index, user.Email, ex.Message);
        }
    }

    private record BulkItemResult(UserDto? User, BulkUserError? Error)
    {
        public static BulkItemResult Ok(UserDto user) => new(user, null);
        public static BulkItemResult Fail(int index, string? email, string reason) =>
            new(null, new BulkUserError(index, email, reason));
    }
}
