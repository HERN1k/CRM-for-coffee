using CRM.Core.Entities;
using CRM.Data.BuildEntitys;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data
{
  public class AppDBContext : DbContext
  {
    public DbSet<EntityUser> Users { get; set; } = null!;
    public DbSet<EntityRefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<EntityProductCategory> ProductCategorys { get; set; } = null!;
    public DbSet<EntityProduct> Products { get; set; } = null!;
    public DbSet<EntityAddOn> AddOns { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      BuilderUser.Build(modelBuilder);
      BuilderRefreshToken.Build(modelBuilder);
      BuilderProductCategory.Build(modelBuilder);
      BuilderProduct.Build(modelBuilder);
      BuilderAddOn.Build(modelBuilder);

    }

    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    { }
  }
}
