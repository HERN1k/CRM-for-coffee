using CRM.Core.Entities;
using CRM.Data.Types;

namespace CRM.Data.Stores
{
  public class ProductStore : IProductStore
  {
    public IQueryable<EntityProductCategory> GetProductCategory(AppDBContext context)
    {

      try
      {
        return context.ProductCategorys;
      }
      catch
      {
        throw;
      }
    }
  }
}