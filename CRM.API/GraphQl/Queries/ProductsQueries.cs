using CRM.Core.Interfaces.Services.BLogicServices.ProductsServices;
using CRM.Core.Models;

using HotChocolate.Authorization;

namespace CRM.API.GraphQl.Queries
{
  public partial class Queries
  {
    [UseProjection]
    [UseSorting()]
    [UseFiltering()]
    [Authorize(Policy = "AdminOrLower")]
    public IQueryable<ProductCategory> GetProductCategories(
        [Service] IProductsService productsService
      ) => productsService.GetProductCategories();

    [UseProjection]
    [UseSorting()]
    [UseFiltering()]
    [Authorize(Policy = "AdminOrLower")]
    public IQueryable<Product> GetProducts(
        [Service] IProductsService productsService
      ) => productsService.GetProducts();

    [UseProjection]
    [UseSorting()]
    [UseFiltering()]
    [Authorize(Policy = "AdminOrLower")]
    public IQueryable<AddOn> GetAddOns(
        [Service] IProductsService productsService
      ) => productsService.GetAddOns();
  }
}