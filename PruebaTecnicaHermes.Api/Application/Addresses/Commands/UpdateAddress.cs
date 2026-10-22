using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Application.Common;
using PruebaTecnicaHermes.Api.Infrastructure;

namespace PruebaTecnicaHermes.Api.Application.Addresses.Commands;

public record UpdateAddressCommand(string Street, string City, string Country, string? ZipCode);

public class UpdateAddressValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressValidator()
    {
        RuleFor(x => x.Street).NotEmpty().WithMessage("La calle es obligatoria.").MaximumLength(200);
        RuleFor(x => x.City).NotEmpty().WithMessage("La ciudad es obligatoria.").MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().WithMessage("El país es obligatorio.").MaximumLength(100);
        RuleFor(x => x.ZipCode).MaximumLength(20);
    }
}

public class UpdateAddressHandler(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<AddressDto> HandleAsync(int id, UpdateAddressCommand command, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var address = await db.Addresses.FirstOrDefaultAsync(a => a.Id == id, ct)
            ?? throw new NotFoundException($"No existe la dirección con id {id}.");

        address.Street = command.Street.Trim();
        address.City = command.City.Trim();
        address.Country = command.Country.Trim();
        address.ZipCode = command.ZipCode?.Trim();

        await db.SaveChangesAsync(ct);

        return AddressDto.FromEntity(address);
    }
}
