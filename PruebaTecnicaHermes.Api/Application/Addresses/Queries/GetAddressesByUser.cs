using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Application.Common;
using PruebaTecnicaHermes.Api.Infrastructure;

namespace PruebaTecnicaHermes.Api.Application.Addresses.Queries;

public class GetAddressesByUserHandler(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<AddressDto>> HandleAsync(int userId, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        if (!await db.Users.AnyAsync(u => u.Id == userId, ct))
            throw new NotFoundException($"No existe el usuario con id {userId}.");

        return await db.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.Id)
            .Select(a => new AddressDto(a.Id, a.UserId, a.Street, a.City, a.Country, a.ZipCode, a.CreatedAt))
            .ToListAsync(ct);
    }
}
