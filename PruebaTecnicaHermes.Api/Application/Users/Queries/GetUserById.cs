using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Application.Common;
using PruebaTecnicaHermes.Api.Infrastructure;

namespace PruebaTecnicaHermes.Api.Application.Users.Queries;

public class GetUserByIdHandler(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<UserDto> HandleAsync(int id, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new NotFoundException($"No existe el usuario con id {id}.");

        return UserDto.FromEntity(user);
    }
}
