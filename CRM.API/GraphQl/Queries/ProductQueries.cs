using CRM.Core.Entities;
using CRM.Data;

namespace CRM.API.GraphQl.Queries
{
  //[Authorize(Policy = "AdminOrLower")]
  public class ProductQueries
  {
    //private readonly IDbContextFactory<AppDBContext> _contextFactory;

    public ProductQueries(
      //IDbContextFactory<AppDBContext> contextFactory
      )
    {
      //_contextFactory = contextFactory;
    }

    [UseProjection]
    [UseSorting()]
    [UseFiltering()]
    public IQueryable<EntityProductCategory> GetCategorys([Service(ServiceKind.Synchronized)] AppDBContext context)
    {
      //await using var context = _contextFactory.CreateDbContext();
      //return context.ProductCategorys;

      return context.ProductCategorys;
    }
  }
}