using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderOrderProduct
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<EntityOrderProduct>()
        .HasMany(e => e.AddOns)
        .WithOne(e => e.OrderProduct)
        .HasForeignKey(e => e.OrderProductId);

      modelBuilder.Entity<EntityOrderProduct>()
        .HasIndex(e => e.Id)
        .IsUnique();

      modelBuilder.Entity<EntityOrderProduct>()
        .Property(e => e.ProductId)
        .IsRequired();
      modelBuilder.Entity<EntityOrderProduct>()
        .Property(e => e.Amount)
        .IsRequired();
      modelBuilder.Entity<EntityOrderProduct>()
        .Property(e => e.OrderId)
        .IsRequired();
    }
  }
}