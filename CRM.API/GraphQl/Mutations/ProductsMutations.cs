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
    public async Task<IEnumerable<EntityProductCategory>> RemoveProductCategory(
        [Service] IProductsService productsService,
        string name
      ) => await productsService.RemoveProductCategory(name);

    [UseProjection]
    public async Task<IEnumerable<EntityProduct>> RemoveProduct(
        [Service] IProductsService productsService,
        string name
      ) => await productsService.RemoveProduct(name);

    [UseProjection]
    public async Task<IEnumerable<EntityAddOn>> RemoveAddOn(
        [Service] IProductsService productsService,
        string name
      ) => await productsService.RemoveAddOn(name);
  }
}