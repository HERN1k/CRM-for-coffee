using AutoMapper;

using CRM.Application.GraphQl.Dto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Entity;
using CRM.Core.Interfaces.Repositories.Base;
using CRM.Core.Interfaces.Repositories.BLogicRepositories.Products;
using CRM.Core.Models;

namespace CRM.Data.Repositories.BLogicRepositories.Products
{
  public class ProductsRepository(
      IBaseRepository repository,
      IMapper mapper
    ) : IProductsRepository
  {
    private readonly IBaseRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public IQueryable<TModel> GetQueryable<TModel, TEntity>() where TModel : class where TEntity : class
    {
      var entities = _repository.GetQueryable<TEntity>();

      var result = new List<TModel>();

      foreach (var entity in entities)
      {
        var temp = _mapper.Map<TModel>(entity);
        result.Add(temp);
      }

      return result.AsQueryable();
    }

    public async Task<IEnumerable<TModel>> GetEnumerable<TModel, TEntity>() where TModel : class where TEntity : class
    {
      var entities = await _repository.GetEnumerable<TEntity>();

      var result = new List<TModel>();

      foreach (var entity in entities)
      {
        var temp = _mapper.Map<TModel>(entity);
        result.Add(temp);
      }

      return result.AsEnumerable();
    }

    public async Task RemoveManyAsync<TModel, TEntity>(List<TModel> removeItems) where TModel : class where TEntity : class
    {
      var removeEntityItems = new List<TEntity>();

      foreach (var items in removeItems)
      {
        var temp = _mapper.Map<TEntity>(items);
        removeEntityItems.Add(temp);
      }

      await _repository.RemoveManyAsync<TEntity>(removeEntityItems);
    }

    public async Task EnsureUniqueName<T>(string name) where T : class, IEntityWithName
    {
      bool thisNewItem = await _repository
        .AnyAsync<T>(e => e.Name == name);
      if (thisNewItem)
        throw new CustomException(ErrorTypes.BadRequest, "This name is already in use");
    }

    public async Task<TModel> GetParentOrThrowAsync<TModel, TEntity>(string parentName) where TModel : class, IEntityWithName where TEntity : class, IEntityWithName
    {
      var entity = await _repository
        .FindSingleAsync<TEntity>(e => e.Name == parentName)
          ?? throw new CustomException(ErrorTypes.BadRequest, "Parent not found");

      var result = _mapper.Map<TModel>(entity);

      return result;
    }

    public async Task<ProductCategory> SaveNewItem(ProductCategoryRequest request)
    {
      var newProductCategory = new EntityProductCategory
      {
        Image = request.Image,
        Name = request.Name
      };
      await _repository.AddAsync<EntityProductCategory>(newProductCategory);

      var result = _mapper.Map<ProductCategory>(newProductCategory);

      return result;
    }

    public async Task<Product> SaveNewItem(ProductCategory parent, ProductRequest request)
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

      var result = _mapper.Map<Product>(newProduct);

      return result;
    }

    public async Task<AddOn> SaveNewItem(Product parent, AddOnRequest request)
    {
      var newAddOn = new EntityAddOn
      {
        ProductId = parent.Id,
        Name = request.Name,
        Price = request.Price,
        Amount = request.Amount
      };
      await _repository.AddAsync<EntityAddOn>(newAddOn);

      var result = _mapper.Map<AddOn>(newAddOn);

      return result;
    }

    public async Task<TModel> FindEntityByNameAsync<TModel, TEntity>(string name) where TModel : class, IEntityWithName where TEntity : class, IEntityWithName
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof(name));

      var entity = await _repository
        .FindSingleAsync<TEntity>(e => e.Name == name)
            ?? throw new CustomException(ErrorTypes.BadRequest, $"Object '{name}' not found");

      var result = _mapper.Map<TModel>(entity);

      return result;
    }
  }
}