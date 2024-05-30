using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderProductCategory
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<EntityProductCategory>()
        .HasMany(e => e.Products)
        .WithOne(e => e.ProductCategory)
        .HasForeignKey(e => e.ProductCategoryId);

      modelBuilder.Entity<EntityProductCategory>()
        .HasIndex(e => e.Name)
        .IsUnique();

      modelBuilder.Entity<EntityProductCategory>()
        .Property(e => e.Name)
        .IsRequired();
      modelBuilder.Entity<EntityProductCategory>()
        .Property(e => e.Image)
        .IsRequired();
    }
  }
}
