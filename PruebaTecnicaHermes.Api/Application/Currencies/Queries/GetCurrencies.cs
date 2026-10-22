using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Infrastructure;

namespace PruebaTecnicaHermes.Api.Application.Currencies.Queries;

public class GetCurrenciesHandler(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<CurrencyDto>> HandleAsync(CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        return await db.Currencies
            .AsNoTracking()
            .OrderBy(c => c.Code)
            .Select(c => new CurrencyDto(c.Id, c.Code, c.Name, c.RateToBase))
            .ToListAsync(ct);
    }
}
