using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderUser
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>()
        .HasOne(e => e.RefreshToken)
        .WithOne(e => e.User)
        .HasForeignKey<RefreshToken>(e => e.UserId);

      modelBuilder.Entity<User>()
        .Property(e => e.Id)
        .IsRequired();
      modelBuilder.Entity<User>()
        .Property(e => e.Password)
        .IsRequired();
      modelBuilder.Entity<User>()
        .Property(e => e.FirstName)
        .IsRequired();
      modelBuilder.Entity<User>()
        .Property(e => e.LastName)
        .IsRequired();
      modelBuilder.Entity<User>()
        .Property(e => e.FatherName)
        .IsRequired();
      modelBuilder.Entity<User>()
        .Property(e => e.Age)
        .IsRequired();
      modelBuilder.Entity<User>()
        .Property(e => e.Gender)
        .IsRequired();
      modelBuilder.Entity<User>()
        .Property(e => e.PhoneNumber)
        .IsRequired();
      modelBuilder.Entity<User>()
        .Property(e => e.Email)
        .IsRequired();
      modelBuilder.Entity<User>()
        .Property(e => e.Post)
        .IsRequired();
      modelBuilder.Entity<User>()
        .Property(e => e.IsConfirmed)
        .IsRequired();
      modelBuilder.Entity<User>()
        .Property(e => e.RegistrationDate)
        .IsRequired();
    }
  }
}
