using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.BuildEntitys
{
  public class BuilderRefreshToken
  {
    public static void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<EntityRefreshToken>()
        .HasKey(e => e.UserId);

      modelBuilder.Entity<EntityRefreshToken>()
        .Property(e => e.UserId)
        .IsRequired();
      modelBuilder.Entity<EntityRefreshToken>()
        .Property(e => e.RefreshTokenString)
        .IsRequired();
    }
  }
}
