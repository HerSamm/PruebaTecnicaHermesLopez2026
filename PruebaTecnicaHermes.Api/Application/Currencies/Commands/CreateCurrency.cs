using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Application.Common;
using PruebaTecnicaHermes.Api.Domain;
using PruebaTecnicaHermes.Api.Infrastructure;

namespace PruebaTecnicaHermes.Api.Application.Currencies.Commands;

public record CreateCurrencyCommand(string Code, string Name, decimal RateToBase);

public class CreateCurrencyValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código es obligatorio.")
            .Length(3).WithMessage("El código debe tener 3 letras (ej.: USD, PYG).");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100);

        RuleFor(x => x.RateToBase)
            .GreaterThan(0).WithMessage("La tasa (rateToBase) debe ser mayor a 0.");
    }
}

public class CreateCurrencyHandler(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<CurrencyDto> HandleAsync(CreateCurrencyCommand command, CancellationToken ct)
    {
        var code = command.Code.Trim().ToUpperInvariant();

        await using var db = await dbFactory.CreateDbContextAsync(ct);

        if (await db.Currencies.AnyAsync(c => c.Code == code, ct))
            throw new ConflictException($"Ya existe una moneda con el código '{code}'.");

        var currency = new Currency
        {
            Code = code,
            Name = command.Name.Trim(),
            RateToBase = command.RateToBase
        };

        db.Currencies.Add(currency);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            throw new ConflictException($"Ya existe una moneda con el código '{code}'.");
        }

        return CurrencyDto.FromEntity(currency);
    }
}
