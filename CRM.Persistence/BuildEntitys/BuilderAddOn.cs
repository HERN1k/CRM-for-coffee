using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderAddOn
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<AddOn>()
        .Property(e => e.Name)
        .IsRequired();
      modelBuilder.Entity<AddOn>()
        .Property(e => e.Price)
        .HasColumnType("numeric(10, 2)")
        .IsRequired();
      modelBuilder.Entity<AddOn>()
        .Property(e => e.Amount)
        .IsRequired();
      modelBuilder.Entity<AddOn>()
        .Property(e => e.ProductId)
        .IsRequired();
    }
  }
}
