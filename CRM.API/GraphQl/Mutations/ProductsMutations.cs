using CRM.Application.GraphQl.Dto;
using CRM.Core.Entities;
using CRM.Core.Interfaces.GraphQl.Mutations;
using CRM.Core.Interfaces.ProductsServices;

namespace CRM.API.GraphQl.Mutations
{
  public class ProductsMutations : IProductsMutations
  {
    [UseProjection]
    public async Task<EntityProductCategory> AddProductCategory(
        [Service] IProductsService productsService,
        ProductCategoryRequest request
      ) => await productsService.SetProductCategory(request);

    [UseProjection]
    public async Task<EntityProduct> AddProduct(
        [Service] IProductsService productsService,
        ProductRequest request
      ) => await productsService.SetProduct(request);

    [UseProjection]
    public async Task<EntityAddOn> AddAddOn(
        [Service] IProductsService productsService,
        AddOnRequest request
      ) => await productsService.SetAddOn(request);

    [UseProjection]
    public async Task<IEnumerable<EntityProductCategory>> RemoveProductCategories(
        [Service] IProductsService productsService,
        params string[] names
      ) => await productsService.RemoveProductCategories(names);

    [UseProjection]
    public async Task<IEnumerable<EntityProduct>> RemoveProducts(
        [Service] IProductsService productsService,
        params string[] names
      ) => await productsService.RemoveProducts(names);

    [UseProjection]
    public async Task<IEnumerable<EntityAddOn>> RemoveAddOns(
        [Service] IProductsService productsService,
        params string[] names
      ) => await productsService.RemoveAddOns(names);
  }
}