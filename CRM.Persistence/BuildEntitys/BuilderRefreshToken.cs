using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderRefreshToken
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<RefreshToken>()
        .HasKey(e => e.UserId);

      modelBuilder.Entity<RefreshToken>()
        .Property(e => e.UserId)
        .IsRequired();
      modelBuilder.Entity<RefreshToken>()
        .Property(e => e.RefreshTokenString)
        .IsRequired();
    }
  }
}
