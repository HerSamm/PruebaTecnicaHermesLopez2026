using Microsoft.EntityFrameworkCore;
using PruebaTecnicaHermes.Api.Domain;

namespace PruebaTecnicaHermes.Api.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasMany(u => u.Addresses)
                  .WithOne(a => a.User)
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.Property(a => a.Street).IsRequired().HasMaxLength(200);
            entity.Property(a => a.City).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Country).IsRequired().HasMaxLength(100);
            entity.Property(a => a.ZipCode).HasMaxLength(20);
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.Property(c => c.Code).IsRequired().HasMaxLength(3);
            entity.HasIndex(c => c.Code).IsUnique();
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.RateToBase).IsRequired();

            entity.HasData(
                new Currency { Id = 1, Code = "PYG", Name = "Guaraní", RateToBase = 1m },
                new Currency { Id = 2, Code = "USD", Name = "Dólar", RateToBase = 6100m },
                new Currency { Id = 3, Code = "EUR", Name = "Euro", RateToBase = 6250m },
                new Currency { Id = 4, Code = "BRL", Name = "Real", RateToBase = 1350m });
        });
    }
}
