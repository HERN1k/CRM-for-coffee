using CRM.Application.GraphQl.Dto;
using CRM.Core.Entities;

namespace CRM.Core.Interfaces.Services.ProductsServices
{
    public interface IProductsService
    {
        IQueryable<EntityProductCategory> GetProductCategories();
        IQueryable<EntityProduct> GetProducts();
        IQueryable<EntityAddOn> GetAddOns();
        Task<EntityProductCategory> SetProductCategoryAsync(ProductCategoryRequest request);
        Task<EntityProduct> SetProductAsync(ProductRequest request);
        Task<EntityAddOn> SetAddOnAsync(AddOnRequest request);
        Task<IEnumerable<EntityProductCategory>> RemoveProductCategoriesAsync(params string[] names);
        Task<IEnumerable<EntityProduct>> RemoveProductsAsync(params string[] names);
        Task<IEnumerable<EntityAddOn>> RemoveAddOnsAsync(params string[] names);
    }
}