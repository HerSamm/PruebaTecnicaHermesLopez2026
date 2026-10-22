using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Application.Common;
using PruebaTecnicaHermes.Api.Infrastructure;

namespace PruebaTecnicaHermes.Api.Application.Users.Commands;

public record UpdateUserCommand(string Name, string Email, bool IsActive);

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(150);
    }
}

public class UpdateUserHandler(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<UserDto> HandleAsync(int id, UpdateUserCommand command, CancellationToken ct)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new NotFoundException($"No existe el usuario con id {id}.");

        if (await db.Users.AnyAsync(u => u.Email == email && u.Id != id, ct))
            throw new ConflictException($"Ya existe otro usuario con el email '{email}'.");

        user.Name = command.Name.Trim();
        user.Email = email;
        user.IsActive = command.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return UserDto.FromEntity(user);
    }
}
