using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderOrderAddOn
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<EntityOrderAddOn>()
        .HasIndex(e => e.Id)
        .IsUnique();

      modelBuilder.Entity<EntityOrderAddOn>()
        .Property(e => e.AddOnId)
        .IsRequired();
      modelBuilder.Entity<EntityOrderAddOn>()
        .Property(e => e.Amount)
        .IsRequired();
      modelBuilder.Entity<EntityOrderAddOn>()
        .Property(e => e.OrderProductId)
        .IsRequired();
    }
  }
}