using CRM.Application.GraphQl.Dto;
using CRM.Core.Models;

namespace CRM.Core.Interfaces.Services.BLogicServices.ProductsServices
{
  /// <summary>
  ///   Provides services for managing products.
  /// </summary>
  public interface IProductsService
  {
    /// <summary>
    ///   Gets a queryable collection of product categories.
    /// </summary>
    /// <returns>A queryable collection of product categories.</returns>
    IQueryable<ProductCategory> GetProductCategories();

    /// <summary>
    ///   Gets a queryable collection of products.
    /// </summary>
    /// <returns>A queryable collection of products.</returns>
    IQueryable<Product> GetProducts();

    /// <summary>
    ///   Gets a queryable collection of add-ons.
    /// </summary>
    /// <returns>A queryable collection of add-ons.</returns>
    IQueryable<AddOn> GetAddOns();

    /// <summary>
    ///   Sets a new product category asynchronously.
    /// </summary>
    /// <param name="request">The product category request.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the new product category.</returns>
    Task<ProductCategory> SetProductCategoryAsync(ProductCategoryRequest request);

    /// <summary>
    ///   Sets a new product asynchronously.
    /// </summary>
    /// <param name="request">The product request.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the new product.</returns>
    Task<Product> SetProductAsync(ProductRequest request);

    /// <summary>
    ///   Sets a new add-on asynchronously.
    /// </summary>
    /// <param name="request">The add-on request.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the new add-on.</returns>
    Task<AddOn> SetAddOnAsync(AddOnRequest request);

    /// <summary>
    ///   Removes multiple product categories asynchronously.
    /// </summary>
    /// <param name="names">The names of the product categories to remove.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the remaining product categories.</returns>
    Task<IEnumerable<ProductCategory>> RemoveProductCategoriesAsync(params string[] names);

    /// <summary>
    ///   Removes multiple products asynchronously.
    /// </summary>
    /// <param name="names">The names of the products to remove.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the remaining products.</returns>
    Task<IEnumerable<Product>> RemoveProductsAsync(params string[] names);

    /// <summary>
    ///   Removes multiple add-ons asynchronously.
    /// </summary>
    /// <param name="names">The names of the add-ons to remove.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the remaining add-ons.</returns>
    Task<IEnumerable<AddOn>> RemoveAddOnsAsync(params string[] names);
  }
}