using CRM.Core.Entities;
using CRM.Data;

namespace CRM.Application.Types
{
  public interface IProductService
  {
    IQueryable<EntityProductCategory> GetProductCategory(AppDBContext context);
  }
}