using Microsoft.EntityFrameworkCore;
using CatalogApi.Domain.Entities;

namespace CatalogApi.Persistence.Data;

public class CatalogApiContext(DbContextOptions<CatalogApiContext> opts) : DbContext(opts) {
  public DbSet<ProductDomain> Products { get; set; } = null!;
  public DbSet<UserDomain> Users { get; set; } = null!;
  public DbSet<CategoryDomain> Categories { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder) {

    modelBuilder.Entity<UserDomain>(static entity => {
      entity.ToTable("Users");
      entity.HasIndex(static e => e.Email).IsUnique();
      entity.HasKey(static e => e.Id).HasName("PK_User");

      entity.HasMany(static e => e.Products)
        .WithOne(static p => p.User)
        .HasForeignKey(static p => p.UserId);
    });

    modelBuilder.Entity<ProductDomain>(static entity => {
      entity.ToTable("Products");
      entity.HasKey(static e => e.Id).HasName("PK_Product");
      entity.HasIndex(static e => e.Name).IsUnique(true);
    });

    modelBuilder.Entity<CategoryDomain>(static entity => {
      entity.ToTable("Categories");
      entity.HasKey(static e => e.Id).HasName("PK_Category");
      entity.HasIndex(static e => e.Name).IsUnique(true);

      entity.HasMany(static e => e.Products).WithMany(static e => e.Categories);
    });
  }
}
