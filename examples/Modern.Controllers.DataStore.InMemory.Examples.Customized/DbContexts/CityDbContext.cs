using Microsoft.EntityFrameworkCore;
using Modern.Controllers.DataStore.InMemory.Examples.Customized.Entities;

namespace Modern.Controllers.DataStore.InMemory.Examples.Customized.DbContexts;

public class CityDbContext : DbContext
{
    public DbSet<CityDbo> Cities { get; set; } = default!;
    
    public CityDbContext(DbContextOptions<CityDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<CityDbo>().ToTable("cities");
        entity.HasKey(x => x.Id);
        entity.HasIndex(x => x.Name);

        entity.Property(x => x.Id).HasColumnName("id").IsRequired();
        entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        entity.Property(x => x.Country).HasColumnName("country").HasMaxLength(100).IsRequired();
        entity.Property(x => x.Area).HasColumnName("area").IsRequired();
        entity.Property(x => x.Population).HasColumnName("population").IsRequired();
    }
}