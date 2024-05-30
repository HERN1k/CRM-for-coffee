using CRM.Application.Types;
using CRM.Core.Entities;
using CRM.Data;
using CRM.Data.Types;

namespace CRM.Application.Services
{
  public class ProductService : IProductService
  {
    private readonly IProductStore _productStore;

    public ProductService(
        IProductStore productStore
      )
    {
      _productStore = productStore;
    }

    public IQueryable<EntityProductCategory> GetProductCategory(AppDBContext context)
    {
      return _productStore.GetProductCategory(context);
    }
  }
}