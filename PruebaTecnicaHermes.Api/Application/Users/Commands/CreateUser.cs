using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Application.Common;
using PruebaTecnicaHermes.Api.Domain;
using PruebaTecnicaHermes.Api.Infrastructure;

namespace PruebaTecnicaHermes.Api.Application.Users.Commands;

public record CreateUserCommand(string Name, string Email, string? Password);

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(150);

        RuleFor(x => x.Password)
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Password));
    }
}

public class CreateUserHandler(IDbContextFactory<AppDbContext> dbFactory, IPasswordHasher<User> passwordHasher)
{
    public async Task<UserDto> HandleAsync(CreateUserCommand command, CancellationToken ct)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        await using var db = await dbFactory.CreateDbContextAsync(ct);

        if (await db.Users.AnyAsync(u => u.Email == email, ct))
            throw new ConflictException($"Ya existe un usuario con el email '{email}'.");

        var user = new User
        {
            Name = command.Name.Trim(),
            Email = email,
            IsActive = true
        };

        if (!string.IsNullOrEmpty(command.Password))
            user.PasswordHash = passwordHasher.HashPassword(user, command.Password);

        db.Users.Add(user);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            throw new ConflictException($"Ya existe un usuario con el email '{email}'.");
        }

        return UserDto.FromEntity(user);
    }
}
