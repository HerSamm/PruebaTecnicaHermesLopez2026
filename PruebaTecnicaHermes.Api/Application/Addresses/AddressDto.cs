using PruebaTecnicaHermes.Api.Domain;

namespace PruebaTecnicaHermes.Api.Application.Addresses;

public record AddressDto(int Id, int UserId, string Street, string City, string Country, string? ZipCode, DateTime CreatedAt)
{
    public static AddressDto FromEntity(Address address) =>
        new(address.Id, address.UserId, address.Street, address.City, address.Country, address.ZipCode, address.CreatedAt);
}
