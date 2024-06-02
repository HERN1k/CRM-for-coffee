using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderUser
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<EntityUser>()
        .HasOne(e => e.RefreshToken)
        .WithOne(e => e.User)
        .HasForeignKey<EntityRefreshToken>(e => e.Id);

      modelBuilder.Entity<EntityUser>()
        .HasIndex(e => e.Email)
        .IsUnique();

      modelBuilder.Entity<EntityUser>()
        .Property(e => e.Id)
        .IsRequired();
      modelBuilder.Entity<EntityUser>()
        .Property(e => e.Password)
        .IsRequired();
      modelBuilder.Entity<EntityUser>()
        .Property(e => e.FirstName)
        .IsRequired();
      modelBuilder.Entity<EntityUser>()
        .Property(e => e.LastName)
        .IsRequired();
      modelBuilder.Entity<EntityUser>()
        .Property(e => e.FatherName)
        .IsRequired();
      modelBuilder.Entity<EntityUser>()
        .Property(e => e.Age)
        .IsRequired();
      modelBuilder.Entity<EntityUser>()
        .Property(e => e.Gender)
        .IsRequired();
      modelBuilder.Entity<EntityUser>()
        .Property(e => e.PhoneNumber)
        .IsRequired();
      modelBuilder.Entity<EntityUser>()
        .Property(e => e.Email)

        .IsRequired();
      modelBuilder.Entity<EntityUser>()
        .Property(e => e.Post)
        .IsRequired();
      modelBuilder.Entity<EntityUser>()
        .Property(e => e.IsConfirmed)
        .IsRequired();
      modelBuilder.Entity<EntityUser>()
        .Property(e => e.RegistrationDate)
        .IsRequired();
    }
  }
}
