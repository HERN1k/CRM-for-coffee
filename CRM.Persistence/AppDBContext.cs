using CRM.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data
{
  public class AppDBContext : DbContext
  {
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
      //Database.Migrate();
    }
  }
}
