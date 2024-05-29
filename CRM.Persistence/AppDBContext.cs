using CRM.Core.Entities;
using CRM.Data.BuildEntitys;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data
{
  public class AppDBContext : DbContext
  {
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<ProductCategory> ProductCategorys { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<AddOn> AddOns { get; set; } = null!;

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
