using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Application.Common;
using PruebaTecnicaHermes.Api.Domain;
using PruebaTecnicaHermes.Api.Infrastructure;

namespace PruebaTecnicaHermes.Api.Application.CurrencyConversion;

public record ConvertCurrencyCommand(string FromCurrencyCode, string ToCurrencyCode, decimal Amount);

public record ConvertCurrencyResult(string FromCurrency, string ToCurrency, decimal OriginalAmount, decimal ConvertedAmount);

public class ConvertCurrencyValidator : AbstractValidator<ConvertCurrencyCommand>
{
    public ConvertCurrencyValidator()
    {
        RuleFor(x => x.FromCurrencyCode).NotEmpty().WithMessage("La moneda de origen es obligatoria.");
        RuleFor(x => x.ToCurrencyCode).NotEmpty().WithMessage("La moneda de destino es obligatoria.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto (amount) debe ser mayor a 0.");
    }
}

public class ConvertCurrencyHandler(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<ConvertCurrencyResult> HandleAsync(ConvertCurrencyCommand command, CancellationToken ct)
    {
        var fromCode = command.FromCurrencyCode.Trim().ToUpperInvariant();
        var toCode = command.ToCurrencyCode.Trim().ToUpperInvariant();
        var fromTask = FindCurrencyAsync(fromCode, ct);
        var toTask = FindCurrencyAsync(toCode, ct);
        await Task.WhenAll(fromTask, toTask);

        var from = await fromTask ?? throw new NotFoundException($"No existe la moneda '{fromCode}'.");
        var to = await toTask ?? throw new NotFoundException($"No existe la moneda '{toCode}'.");

        var baseAmount = command.Amount * from.RateToBase;
        var convertedAmount = Math.Round(baseAmount / to.RateToBase, 2);

        return new ConvertCurrencyResult(from.Code, to.Code, command.Amount, convertedAmount);
    }

    private async Task<Currency?> FindCurrencyAsync(string code, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Currencies.AsNoTracking().FirstOrDefaultAsync(c => c.Code == code, ct);
    }
}
