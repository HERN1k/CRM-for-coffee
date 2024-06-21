using CRM.Application.GraphQl.Dto;
using CRM.Core.Entities;
using CRM.Core.Interfaces.ProductsServices;

using HotChocolate.Authorization;

namespace CRM.API.GraphQl.Mutations
{
  public partial class Mutations
  {
    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<EntityProductCategory> AddProductCategory(
        [Service] IProductsService productsService,
        ProductCategoryRequest request
      ) => await productsService.SetProductCategory(request);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<EntityProduct> AddProduct(
        [Service] IProductsService productsService,
        ProductRequest request
      ) => await productsService.SetProduct(request);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<EntityAddOn> AddAddOn(
        [Service] IProductsService productsService,
        AddOnRequest request
      ) => await productsService.SetAddOn(request);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<IEnumerable<EntityProductCategory>> RemoveProductCategories(
        [Service] IProductsService productsService,
        params string[] names
      ) => await productsService.RemoveProductCategories(names);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<IEnumerable<EntityProduct>> RemoveProducts(
        [Service] IProductsService productsService,
        params string[] names
      ) => await productsService.RemoveProducts(names);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<IEnumerable<EntityAddOn>> RemoveAddOns(
        [Service] IProductsService productsService,
        params string[] names
      ) => await productsService.RemoveAddOns(names);
  }
}