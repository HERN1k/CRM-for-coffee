using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderAddOn
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<EntityAddOn>()
        .Property(e => e.Name)
        .IsRequired();
      modelBuilder.Entity<EntityAddOn>()
        .Property(e => e.Price)
        .HasColumnType("numeric(10, 2)")
        .IsRequired();
      modelBuilder.Entity<EntityAddOn>()
        .Property(e => e.Amount)
        .IsRequired();
      modelBuilder.Entity<EntityAddOn>()
        .Property(e => e.ProductId)
        .IsRequired();
      modelBuilder.Entity<EntityAddOn>()
        .Ignore(e => e.Key);
    }
  }
}
