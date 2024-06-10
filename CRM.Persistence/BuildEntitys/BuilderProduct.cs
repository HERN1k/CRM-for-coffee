using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderProduct
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<EntityProduct>()
        .HasMany(e => e.AddOns)
        .WithOne(e => e.Product)
        .HasForeignKey(e => e.ProductId);

      modelBuilder.Entity<EntityProduct>()
        .HasIndex(e => e.Id)
        .IsUnique();

      modelBuilder.Entity<EntityProduct>()
        .Property(e => e.Name)
        .IsRequired();
      modelBuilder.Entity<EntityProduct>()
        .Property(e => e.Image)
        .IsRequired();
      modelBuilder.Entity<EntityProduct>()
        .Property(e => e.Price)
        .HasColumnType("numeric(10, 2)")
        .IsRequired();
      modelBuilder.Entity<EntityProduct>()
        .Property(e => e.Amount)
        .IsRequired();
      modelBuilder.Entity<EntityProduct>()
        .Property(e => e.ProductCategoryId)
        .IsRequired();
    }
  }
}