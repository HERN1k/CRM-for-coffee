using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderProductCategory
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ProductCategory>()
        .HasMany(e => e.Products)
        .WithOne(e => e.ProductCategory)
        .HasForeignKey(e => e.ProductCategoryId);
      //modelBuilder.Entity<ProductCategory>()
      //  .HasIndex(e => e.Name)
      //  .IsUnique();
      //modelBuilder.Entity<ProductCategory>()
      //  .HasKey(e => e.Name);

      modelBuilder.Entity<ProductCategory>()
        .Property(e => e.Name)
        .IsRequired();
      modelBuilder.Entity<ProductCategory>()
        .Property(e => e.Image)
        .IsRequired();
    }
  }
}
