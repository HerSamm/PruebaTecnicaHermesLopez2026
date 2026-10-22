using PruebaTecnicaHermes.Api.Domain;

namespace PruebaTecnicaHermes.Api.Application.Currencies;

public record CurrencyDto(int Id, string Code, string Name, decimal RateToBase)
{
    public static CurrencyDto FromEntity(Currency currency) =>
        new(currency.Id, currency.Code, currency.Name, currency.RateToBase);
}
