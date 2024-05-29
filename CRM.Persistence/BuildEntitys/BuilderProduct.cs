using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderProduct
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Product>()
        .HasMany(e => e.AddOns)
        .WithOne(e => e.Product)
        .HasForeignKey(e => e.ProductId);
      //modelBuilder.Entity<Product>()
      //  .HasIndex(e => e.Name)
      //  .IsUnique();
      //modelBuilder.Entity<Product>()
      //  .HasKey(e => e.Name);

      modelBuilder.Entity<Product>()
        .Property(e => e.Name)
        .IsRequired();
      modelBuilder.Entity<Product>()
        .Property(e => e.Image)
        .IsRequired();
      modelBuilder.Entity<Product>()
        .Property(e => e.Price)
        .HasColumnType("numeric(10, 2)")
        .IsRequired();
      modelBuilder.Entity<Product>()
        .Property(e => e.Amount)
        .IsRequired();
      modelBuilder.Entity<Product>()
        .Property(e => e.ProductCategoryId)
        .IsRequired();
    }
  }
}
