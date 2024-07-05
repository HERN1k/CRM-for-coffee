using CRM.Application.GraphQl.Dto;
using CRM.Application.Tools.RequestValidation;
using CRM.Core.Entities;
using CRM.Core.Interfaces.Repositories.Base;
using CRM.Core.Interfaces.Repositories.BLogicRepositories.Products;
using CRM.Core.Interfaces.Services.BLogicServices.ProductsServices;
using CRM.Core.Models;

namespace CRM.Application.Services.ProductsServices
{
  public class ProductsService(
      IProductsRepository repository,
      IProductsComponents components,

      IBaseRepository rep
    ) : IProductsService
  {
    private readonly IProductsRepository _repository = repository;
    private readonly IProductsComponents _components = components;

    private readonly IBaseRepository _rep = rep;

    public IQueryable<ProductCategory> GetProductCategories() =>
      _repository.GetQueryable<ProductCategory, EntityProductCategory>();

    public IQueryable<Product> GetProducts() =>
      _repository.GetQueryable<Product, EntityProduct>();

    public IQueryable<AddOn> GetAddOns() =>
      _repository.GetQueryable<AddOn, EntityAddOn>();

    public async Task<ProductCategory> SetProductCategoryAsync(ProductCategoryRequest request)
    {
      RequestValidator.Validate(request);

      await _repository.EnsureUniqueName<EntityProductCategory>(request.Name);

      var newProductCategory = await _repository.SaveNewItem(request);

      return newProductCategory;
    }

    public async Task<Product> SetProductAsync(ProductRequest request)
    {
      RequestValidator.Validate(request);

      await _repository.EnsureUniqueName<EntityProduct>(request.Name);

      var parent = await _repository.GetParentOrThrowAsync<ProductCategory, EntityProductCategory>(request.CategoryName);

      var newProduct = await _repository.SaveNewItem(parent, request);

      return newProduct;
    }

    public async Task<AddOn> SetAddOnAsync(AddOnRequest request)
    {
      RequestValidator.Validate(request);

      var parent = await _repository.GetParentOrThrowAsync<Product, EntityProduct>(request.ProductName);

      var newAddOn = await _repository.SaveNewItem(parent, request);

      return newAddOn;
    }

    public async Task<IEnumerable<ProductCategory>> RemoveProductCategoriesAsync(params string[] names)
    {
      _components.ValidateObjectCountForDeletion(names);

      RequestValidator.ValidateProductsName(names);

      var removeItems = await _components.GetRemoveItems<ProductCategory, EntityProductCategory>(names);

      await _repository.RemoveManyAsync<ProductCategory, EntityProductCategory>(removeItems);

      return await _repository.GetEnumerable<ProductCategory, EntityProductCategory>();
    }

    public async Task<IEnumerable<Product>> RemoveProductsAsync(params string[] names)
    {
      _components.ValidateObjectCountForDeletion(names);

      RequestValidator.ValidateProductsName(names);

      var removeItems = await _components.GetRemoveItems<Product, EntityProduct>(names);

      await _repository.RemoveManyAsync<Product, EntityProduct>(removeItems);

      return await _repository.GetEnumerable<Product, EntityProduct>();
    }

    public async Task<IEnumerable<AddOn>> RemoveAddOnsAsync(params string[] names)
    {
      _components.ValidateObjectCountForDeletion(names);

      RequestValidator.ValidateProductsName(names);

      var removeItems = await _components.GetRemoveItems<AddOn, EntityAddOn>(names);

      await _repository.RemoveManyAsync<AddOn, EntityAddOn>(removeItems);

      return await _repository.GetEnumerable<AddOn, EntityAddOn>();
    }
  }
}