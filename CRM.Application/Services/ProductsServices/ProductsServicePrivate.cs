using CRM.Application.GraphQl.Dto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.Entity;

namespace CRM.Application.Services.ProductsServices
{
  public partial class ProductsService
  {
    private async Task EnsureUniqueName<T>(string name) where T : class, IEntityWithName
    {
      bool thisNewItem = await _repository
        .AnyAsync<T>(e => e.Name == name);
      if (thisNewItem)
        throw new CustomException(ErrorTypes.BadRequest, "This name is already in use");
    }
    private async Task<T> GetParentOrThrowAsync<T>(string parentName) where T : class, IEntityWithName
    {
      var parent = await _repository
        .FindSingleAsync<T>(e => e.Name == parentName)
          ?? throw new CustomException(ErrorTypes.BadRequest, "Parent not found");
      return parent;
    }
    private async Task<EntityProductCategory> SaveNewItem(ProductCategoryRequest request)
    {
      var newProductCategory = new EntityProductCategory
      {
        Image = request.Image,
        Name = request.Name
      };
      await _repository.AddAsync<EntityProductCategory>(newProductCategory);
      return newProductCategory;
    }
    private async Task<EntityProduct> SaveNewItem(EntityProductCategory parent, ProductRequest request)
    {
      var newProduct = new EntityProduct
      {
        ProductCategoryId = parent.Id,
        Name = request.Name,
        Image = request.Image,
        Price = request.Price,
        Amount = request.Amount
      };
      await _repository.AddAsync<EntityProduct>(newProduct);
      return newProduct;
    }
    private async Task<EntityAddOn> SaveNewItem(EntityProduct parent, AddOnRequest request)
    {
      var newAddOn = new EntityAddOn
      {
        ProductId = parent.Id,
        Name = request.Name,
        Price = request.Price,
        Amount = request.Amount
      };
      await _repository.AddAsync<EntityAddOn>(newAddOn);
      return newAddOn;
    }
    private void ValidateObjectCountForDeletion(params string[] names)
    {
      if (names.Length <= 0)
        throw new CustomException(ErrorTypes.BadRequest, $"Objects not found");
      if (names.Length > 10)
        throw new CustomException(ErrorTypes.BadRequest, $"You can delete up to 10 objects at a time");
    }
    private async Task<List<T>> GetRemoveItems<T>(params string[] names) where T : class, IEntityWithName
    {
      var result = new List<T>();
      foreach (var name in names)
      {
        var entity = await _repository
          .FindSingleAsync<T>(e => e.Name == name)
            ?? throw new CustomException(ErrorTypes.BadRequest, $"Object '{name}' not found");
        result.Add(entity);
      }
      return result;
    }
  }
}