using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Application.Common;
using PruebaTecnicaHermes.Api.Infrastructure;

namespace PruebaTecnicaHermes.Api.Application.Addresses.Commands;

public class DeleteAddressHandler(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task HandleAsync(int id, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var address = await db.Addresses.FirstOrDefaultAsync(a => a.Id == id, ct)
            ?? throw new NotFoundException($"No existe la dirección con id {id}.");

        db.Addresses.Remove(address);
        await db.SaveChangesAsync(ct);
    }
}
