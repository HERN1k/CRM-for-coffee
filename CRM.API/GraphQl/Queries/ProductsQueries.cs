using CRM.Core.Entities;
using CRM.Core.Interfaces.GraphQl.Queries;
using CRM.Core.Interfaces.ProductsServices;

namespace CRM.API.GraphQl.Queries
{
  //[Authorize(Policy = "AdminOrLower")]
  public class ProductsQueries : IProductsQueries
  {
    [UseProjection]
    [UseSorting()]
    [UseFiltering()]
    public IQueryable<EntityProductCategory> GetProductCategories(
        [Service] IProductsService productsService
      ) => productsService.GetProductCategories();

    [UseProjection]
    [UseSorting()]
    [UseFiltering()]
    public IQueryable<EntityProduct> GetProducts(
        [Service] IProductsService productsService
      ) => productsService.GetProducts();

    [UseProjection]
    [UseSorting()]
    [UseFiltering()]
    public IQueryable<EntityAddOn> GetAddOns(
        [Service] IProductsService productsService
      ) => productsService.GetAddOns();
  }
}