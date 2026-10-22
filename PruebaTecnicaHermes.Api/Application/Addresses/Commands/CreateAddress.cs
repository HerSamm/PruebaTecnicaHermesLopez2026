using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Application.Common;
using PruebaTecnicaHermes.Api.Domain;
using PruebaTecnicaHermes.Api.Infrastructure;

namespace PruebaTecnicaHermes.Api.Application.Addresses.Commands;

public record CreateAddressCommand(string Street, string City, string Country, string? ZipCode);

public class CreateAddressValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressValidator()
    {
        RuleFor(x => x.Street).NotEmpty().WithMessage("La calle es obligatoria.").MaximumLength(200);
        RuleFor(x => x.City).NotEmpty().WithMessage("La ciudad es obligatoria.").MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().WithMessage("El país es obligatorio.").MaximumLength(100);
        RuleFor(x => x.ZipCode).MaximumLength(20);
    }
}

public class CreateAddressHandler(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<AddressDto> HandleAsync(int userId, CreateAddressCommand command, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        if (!await db.Users.AnyAsync(u => u.Id == userId, ct))
            throw new NotFoundException($"No existe el usuario con id {userId}.");

        var address = new Address
        {
            UserId = userId,
            Street = command.Street.Trim(),
            City = command.City.Trim(),
            Country = command.Country.Trim(),
            ZipCode = command.ZipCode?.Trim()
        };

        db.Addresses.Add(address);
        await db.SaveChangesAsync(ct);

        return AddressDto.FromEntity(address);
    }
}
