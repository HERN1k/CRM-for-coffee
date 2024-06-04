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
    Task<IEnumerable<EntityProductCategory>> RemoveProductCategory(string name);
    Task<IEnumerable<EntityProduct>> RemoveProduct(string name);
    Task<IEnumerable<EntityAddOn>> RemoveAddOn(string name);
  }
}