using FluentValidation;
using Microsoft.AspNetCore.Identity;
using PruebaTecnicaHermes.Api.Application.Addresses.Commands;
using PruebaTecnicaHermes.Api.Application.Addresses.Queries;
using PruebaTecnicaHermes.Api.Application.Currencies.Commands;
using PruebaTecnicaHermes.Api.Application.Currencies.Queries;
using PruebaTecnicaHermes.Api.Application.CurrencyConversion;
using PruebaTecnicaHermes.Api.Application.Users.Commands;
using PruebaTecnicaHermes.Api.Application.Users.Queries;
using PruebaTecnicaHermes.Api.Domain;

namespace PruebaTecnicaHermes.Api.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        
        services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        // Users
        services.AddScoped<CreateUserHandler>();
        services.AddScoped<UpdateUserHandler>();
        services.AddScoped<DeleteUserHandler>();
        services.AddScoped<BulkCreateUsersHandler>();
        services.AddScoped<GetUsersHandler>();
        services.AddScoped<GetUserByIdHandler>();

        // Addresses
        services.AddScoped<CreateAddressHandler>();
        services.AddScoped<UpdateAddressHandler>();
        services.AddScoped<DeleteAddressHandler>();
        services.AddScoped<GetAddressesByUserHandler>();

        // Currencies
        services.AddScoped<CreateCurrencyHandler>();
        services.AddScoped<GetCurrenciesHandler>();
        services.AddScoped<ConvertCurrencyHandler>();

        return services;
    }
}
