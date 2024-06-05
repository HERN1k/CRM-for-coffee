using CRM.Application.GraphQl.Dto;
using CRM.Core.Entities;

namespace CRM.Core.Interfaces.ProductsServices
{
  public interface IProductsService
  {
    IQueryable<EntityProductCategory> GetProductCategories();
    IQueryable<EntityProduct> GetProducts();
    IQueryable<EntityAddOn> GetAddOns();
    Task<EntityProductCategory> SetProductCategory(ProductCategoryRequest request);
    Task<EntityProduct> SetProduct(ProductRequest request);
    Task<EntityAddOn> SetAddOn(AddOnRequest request);
    Task<IEnumerable<EntityProductCategory>> RemoveProductCategories(params string[] names);
    Task<IEnumerable<EntityProduct>> RemoveProducts(params string[] names);
    Task<IEnumerable<EntityAddOn>> RemoveAddOns(params string[] names);
  }
}