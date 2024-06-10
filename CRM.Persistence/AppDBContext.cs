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
    public DbSet<EntityOrder> Orders { get; set; } = null!;
    public DbSet<EntityOrderProduct> OrderProducts { get; set; } = null!;
    public DbSet<EntityOrderAddOn> OrderAddOns { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      BuilderUser.Build(modelBuilder);
      BuilderRefreshToken.Build(modelBuilder);
      BuilderProductCategory.Build(modelBuilder);
      BuilderProduct.Build(modelBuilder);
      BuilderAddOn.Build(modelBuilder);
      BuilderOrder.Build(modelBuilder);
      BuilderOrderProduct.Build(modelBuilder);
      BuilderOrderAddOn.Build(modelBuilder);

    }

    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    { }
  }
}