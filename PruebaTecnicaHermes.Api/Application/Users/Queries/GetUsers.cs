using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Infrastructure;

namespace PruebaTecnicaHermes.Api.Application.Users.Queries;

public record GetUsersQuery(bool? IsActive);

public class GetUsersHandler(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<UserDto>> HandleAsync(GetUsersQuery query, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var users = db.Users.AsNoTracking();

        if (query.IsActive.HasValue)
            users = users.Where(u => u.IsActive == query.IsActive.Value);

        return await users
            .OrderBy(u => u.Id)
            .Select(u => new UserDto(u.Id, u.Name, u.Email, u.IsActive, u.CreatedAt, u.UpdatedAt))
            .ToListAsync(ct);
    }
}
