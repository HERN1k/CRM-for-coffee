using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderOrder
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<EntityOrder>()
        .HasMany(e => e.Products)
        .WithOne(e => e.Order)
        .HasForeignKey(e => e.OrderId);

      modelBuilder.Entity<EntityOrder>()
        .HasIndex(e => e.Id)
        .IsUnique();

      modelBuilder.Entity<EntityOrder>()
        .Property(e => e.Taxes)
        .HasColumnType("numeric(10, 2)")
        .IsRequired();
      modelBuilder.Entity<EntityOrder>()
        .Property(e => e.Total)
        .HasColumnType("numeric(10, 2)")
        .IsRequired();
      modelBuilder.Entity<EntityOrder>()
        .Property(e => e.PaymentMethod)
        .IsRequired();
      modelBuilder.Entity<EntityOrder>()
        .Property(e => e.WorkerId)
        .IsRequired();
      modelBuilder.Entity<EntityOrder>()
        .Property(e => e.OrderСreationDate)
        .IsRequired();
      modelBuilder.Entity<EntityOrder>()
        .Ignore(e => e.OrderNumber);
    }
  }
}