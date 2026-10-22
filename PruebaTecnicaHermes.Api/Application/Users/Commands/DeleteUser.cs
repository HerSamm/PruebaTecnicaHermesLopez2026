using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Application.Common;
using PruebaTecnicaHermes.Api.Infrastructure;

namespace PruebaTecnicaHermes.Api.Application.Users.Commands;

public class DeleteUserHandler(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task HandleAsync(int id, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new NotFoundException($"No existe el usuario con id {id}.");
     
        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
    }
}
