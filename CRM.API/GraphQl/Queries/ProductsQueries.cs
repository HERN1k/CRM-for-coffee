using CRM.Core.Entities;
using CRM.Core.Interfaces.ProductsServices;

using HotChocolate.Authorization;

namespace CRM.API.GraphQl.Queries
{
  public partial class Queries
  {
    [UseProjection]
    [UseSorting()]
    [UseFiltering()]
    [Authorize(Policy = "AdminOrLower")]
    public IQueryable<EntityProductCategory> GetProductCategories(
        [Service] IProductsService productsService
      ) => productsService.GetProductCategories();

    [UseProjection]
    [UseSorting()]
    [UseFiltering()]
    [Authorize(Policy = "AdminOrLower")]
    public IQueryable<EntityProduct> GetProducts(
        [Service] IProductsService productsService
      ) => productsService.GetProducts();

    [UseProjection]
    [UseSorting()]
    [UseFiltering()]
    [Authorize(Policy = "AdminOrLower")]
    public IQueryable<EntityAddOn> GetAddOns(
        [Service] IProductsService productsService
      ) => productsService.GetAddOns();
  }
}