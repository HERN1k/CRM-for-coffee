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
    Task<IEnumerable<EntityProductCategory>> RemoveProductCategory(
        [Service] IProductsService productsService,
        string name
      );
    Task<IEnumerable<EntityProduct>> RemoveProduct(
        [Service] IProductsService productsService,
        string name
      );
    Task<IEnumerable<EntityAddOn>> RemoveAddOn(
        [Service] IProductsService productsService,
        string name
      );
  }
}