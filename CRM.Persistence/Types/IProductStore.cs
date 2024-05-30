using CRM.Core.Entities;

namespace CRM.Data.Types
{
  public interface IProductStore
  {
    IQueryable<EntityProductCategory> GetProductCategory(AppDBContext context);
  }
}