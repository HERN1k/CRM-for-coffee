using CRM.Application.GraphQl.Dto;
using CRM.Core.Interfaces.Services.BLogicServices.ProductsServices;
using CRM.Core.Models;

using HotChocolate.Authorization;

namespace CRM.API.GraphQl.Mutations
{
  public partial class Mutations
  {
    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<ProductCategory> AddProductCategory(
        [Service] IProductsService productsService,
        ProductCategoryRequest request
      ) => await productsService.SetProductCategoryAsync(request);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<Product> AddProduct(
        [Service] IProductsService productsService,
        ProductRequest request
      ) => await productsService.SetProductAsync(request);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<AddOn> AddAddOn(
        [Service] IProductsService productsService,
        AddOnRequest request
      ) => await productsService.SetAddOnAsync(request);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<IEnumerable<ProductCategory>> RemoveProductCategories(
        [Service] IProductsService productsService,
        params string[] names
      ) => await productsService.RemoveProductCategoriesAsync(names);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<IEnumerable<Product>> RemoveProducts(
        [Service] IProductsService productsService,
        params string[] names
      ) => await productsService.RemoveProductsAsync(names);

    [UseProjection]
    [Authorize(Policy = "ManagerOrUpper")]
    public async Task<IEnumerable<AddOn>> RemoveAddOns(
        [Service] IProductsService productsService,
        params string[] names
      ) => await productsService.RemoveAddOnsAsync(names);
  }
}