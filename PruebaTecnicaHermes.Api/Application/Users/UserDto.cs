using PruebaTecnicaHermes.Api.Domain;

namespace PruebaTecnicaHermes.Api.Application.Users;

public record UserDto(int Id, string Name, string Email, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt)
{
    public static UserDto FromEntity(User user) =>
        new(user.Id, user.Name, user.Email, user.IsActive, user.CreatedAt, user.UpdatedAt);
}
