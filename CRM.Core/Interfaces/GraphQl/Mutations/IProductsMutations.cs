using CRM.Application.GraphQl.Dto;
using CRM.Core.Entities;
using CRM.Core.Interfaces.ProductsServices;

using HotChocolate;

namespace CRM.Core.Interfaces.GraphQl.Mutations
{
  public interface IProductsMutations
  {
    Task<EntityProductCategory> AddProductCategory(
        [Service] IProductsService productsService,
        ProductCategoryRequest request
      );
    Task<EntityProduct> AddProduct(
        [Service] IProductsService productsService,
        ProductRequest request
      );
    Task<EntityAddOn> AddAddOn(
        [Service] IProductsService productsService,
        AddOnRequest request
      );
    Task<IEnumerable<EntityProductCategory>> RemoveProductCategories(
        [Service] IProductsService productsService,
        params string[] names
      );
    Task<IEnumerable<EntityProduct>> RemoveProducts(
        [Service] IProductsService productsService,
        params string[] names
      );
    Task<IEnumerable<EntityAddOn>> RemoveAddOns(
        [Service] IProductsService productsService,
        params string[] names
      );
  }
}