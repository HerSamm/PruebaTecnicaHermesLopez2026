using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PruebaTecnicaHermes.Api.Application;
using PruebaTecnicaHermes.Api.Endpoints;
using PruebaTecnicaHermes.Api.Infrastructure;
using PruebaTecnicaHermes.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddApplication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Prueba Técnica Hermes - Users, Addresses y Currencies",
        Version = "v1"
    });

    
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = ApiKeyMiddleware.HeaderName,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "API Key requerida en todas las peticiones."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

await using (var db = await app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContextAsync())
{
    await db.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options => options.EnablePersistAuthorization());

app.UseMiddleware<ApiKeyMiddleware>();

app.MapUserEndpoints();
app.MapAddressEndpoints();
app.MapCurrencyEndpoints();

app.Run();
