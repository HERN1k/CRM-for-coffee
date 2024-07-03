using CRM.Application.GraphQl.Dto;
using CRM.Core.Entities;
using CRM.Core.Interfaces.Services.ProductsServices;
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
      ) => await productsService.SetProductCategoryAsync(request);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<EntityProduct> AddProduct(
        [Service] IProductsService productsService,
        ProductRequest request
      ) => await productsService.SetProductAsync(request);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<EntityAddOn> AddAddOn(
        [Service] IProductsService productsService,
        AddOnRequest request
      ) => await productsService.SetAddOnAsync(request);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<IEnumerable<EntityProductCategory>> RemoveProductCategories(
        [Service] IProductsService productsService,
        params string[] names
      ) => await productsService.RemoveProductCategoriesAsync(names);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<IEnumerable<EntityProduct>> RemoveProducts(
        [Service] IProductsService productsService,
        params string[] names
      ) => await productsService.RemoveProductsAsync(names);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<IEnumerable<EntityAddOn>> RemoveAddOns(
        [Service] IProductsService productsService,
        params string[] names
      ) => await productsService.RemoveAddOnsAsync(names);
  }
}