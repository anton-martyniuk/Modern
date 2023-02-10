using Microsoft.EntityFrameworkCore;
using Modern.Services.DataStore.InMemory.Examples.Entities;

namespace Modern.Services.DataStore.InMemory.Examples.DbContexts;

public class CityDbContext : DbContext
{
    public DbSet<CityDbo> Cities { get; set; } = default!;
    
    public CityDbContext(DbContextOptions<CityDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var flyingCompany = modelBuilder.Entity<CityDbo>().ToTable("cities");
        flyingCompany.HasKey(x => x.Id);
        flyingCompany.HasIndex(x => x.Name).IsUnique();

        flyingCompany.Property(x => x.Id).HasColumnName("id").IsRequired();
        flyingCompany.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        flyingCompany.Property(x => x.Country).HasColumnName("country").HasMaxLength(100).IsRequired();
        flyingCompany.Property(x => x.Area).HasColumnName("area").IsRequired();
        flyingCompany.Property(x => x.Population).HasColumnName("population").IsRequired();
    }
}