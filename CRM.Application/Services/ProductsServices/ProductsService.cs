using CRM.Application.GraphQl.Dto;
using CRM.Application.RequestValidation;
using CRM.Core.Entities;
using CRM.Core.Interfaces.ProductsServices;
using CRM.Core.Interfaces.Repositories;

namespace CRM.Application.Services.ProductsServices
{
  public partial class ProductsService(
      IRepository repository
    ) : IProductsService
  {
    private readonly IRepository _repository = repository;

    public IQueryable<EntityProductCategory> GetProductCategories() =>
      _repository.GetQueryable<EntityProductCategory>();

    public IQueryable<EntityProduct> GetProducts() =>
      _repository.GetQueryable<EntityProduct>();

    public IQueryable<EntityAddOn> GetAddOns() =>
      _repository.GetQueryable<EntityAddOn>();

    public async Task<EntityProductCategory> SetProductCategoryAsync(ProductCategoryRequest request)
    {
      RequestValidator.Validate(request);

      await EnsureUniqueName<EntityProductCategory>(request.Name);

      var newProductCategory = await SaveNewItem(request);

      return newProductCategory;
    }

    public async Task<EntityProduct> SetProductAsync(ProductRequest request)
    {
      RequestValidator.Validate(request);

      await EnsureUniqueName<EntityProduct>(request.Name);

      var parent = await GetParentOrThrowAsync<EntityProductCategory>(request.CategoryName);

      var newProduct = await SaveNewItem(parent, request);

      return newProduct;
    }

    public async Task<EntityAddOn> SetAddOnAsync(AddOnRequest request)
    {
      RequestValidator.Validate(request);

      var parent = await GetParentOrThrowAsync<EntityProduct>(request.ProductName);

      var newAddOn = await SaveNewItem(parent, request);

      return newAddOn;
    }

    public async Task<IEnumerable<EntityProductCategory>> RemoveProductCategoriesAsync(params string[] names)
    {
      ValidateObjectCountForDeletion(names);

      RequestValidator.ValidateProductsName(names);

      var removeItems = await GetRemoveItems<EntityProductCategory>(names);

      await _repository.RemoveManyAsync<EntityProductCategory>(removeItems);

      return await _repository.GetEnumerable<EntityProductCategory>();
    }

    public async Task<IEnumerable<EntityProduct>> RemoveProductsAsync(params string[] names)
    {
      ValidateObjectCountForDeletion(names);

      RequestValidator.ValidateProductsName(names);

      var removeItems = await GetRemoveItems<EntityProduct>(names);

      await _repository.RemoveManyAsync<EntityProduct>(removeItems);

      return await _repository.GetEnumerable<EntityProduct>();
    }

    public async Task<IEnumerable<EntityAddOn>> RemoveAddOnsAsync(params string[] names)
    {
      ValidateObjectCountForDeletion(names);

      RequestValidator.ValidateProductsName(names);

      var removeItems = await GetRemoveItems<EntityAddOn>(names);

      await _repository.RemoveManyAsync<EntityAddOn>(removeItems);

      return await _repository.GetEnumerable<EntityAddOn>();
    }
  }
}