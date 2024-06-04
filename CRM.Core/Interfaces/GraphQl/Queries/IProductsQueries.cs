using CRM.Core.Entities;
using CRM.Core.Interfaces.ProductsServices;

using HotChocolate;

namespace CRM.Core.Interfaces.GraphQl.Queries
{
  public interface IProductsQueries
  {
    IQueryable<EntityProductCategory> GetProductCategories([Service] IProductsService productsService);
    IQueryable<EntityProduct> GetProducts([Service] IProductsService productsService);
    IQueryable<EntityAddOn> GetAddOns([Service] IProductsService productsService);
  }
}