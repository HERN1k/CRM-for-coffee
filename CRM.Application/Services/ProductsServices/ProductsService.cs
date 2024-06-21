using CRM.Application.GraphQl.Dto;
using CRM.Application.RegEx;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.ProductsServices;
using CRM.Core.Interfaces.Repositories;

namespace CRM.Application.Services.ProductsServices
{
  public class ProductsService : IProductsService
  {
    private readonly IRepository _repository;

    public ProductsService(
        IRepository repository
      )
    {
      _repository = repository;
    }

    #region GetProductCategories
    public IQueryable<EntityProductCategory> GetProductCategories() =>
      _repository.GetQueryable<EntityProductCategory>();
    #endregion

    #region GetProducts
    public IQueryable<EntityProduct> GetProducts() =>
      _repository.GetQueryable<EntityProduct>();
    #endregion

    #region GetAddOns
    public IQueryable<EntityAddOn> GetAddOns() =>
      _repository.GetQueryable<EntityAddOn>();
    #endregion

    #region SetProductCategory
    public async Task<EntityProductCategory> SetProductCategory(ProductCategoryRequest request)
    {
      bool isValidName = RegExHelper.ChackString(request.Name, RegExPatterns.ProductName);
      if (!isValidName)
        throw new CustomException(ErrorTypes.ValidationError, "Name is incorrect or null");

      bool isValidImage = RegExHelper.ChackString(request.Image, RegExPatterns.URL);
      if (!isValidImage)
        throw new CustomException(ErrorTypes.ValidationError, "Image is incorrect or null");

      bool thisNewItem = await _repository.AnyAsync<EntityProductCategory>(e => e.Name == request.Name);
      if (thisNewItem)
        throw new CustomException(ErrorTypes.BadRequest, "This name is already in use");

      var newProductCategory = new EntityProductCategory
      {
        Image = request.Image,
        Name = request.Name
      };

      await _repository.AddAsync<EntityProductCategory>(newProductCategory);

      return newProductCategory;
    }
    #endregion

    #region SetProduct
    public async Task<EntityProduct> SetProduct(ProductRequest request)
    {
      bool isValidName = RegExHelper.ChackString(request.Name, RegExPatterns.ProductName);
      if (!isValidName)
        throw new CustomException(ErrorTypes.ValidationError, "Name is incorrect or null");

      bool isValidCategoryName = RegExHelper.ChackString(request.CategoryName, RegExPatterns.ProductName);
      if (!isValidCategoryName)
        throw new CustomException(ErrorTypes.ValidationError, "Image is incorrect or null");

      bool isValidImage = RegExHelper.ChackString(request.Image, RegExPatterns.URL);
      if (!isValidImage)
        throw new CustomException(ErrorTypes.ValidationError, "Image is incorrect or null");

      if (request.Price < 0)
        throw new CustomException(ErrorTypes.ValidationError, "Price is incorrect or null");

      if (request.Amount < 1)
        throw new CustomException(ErrorTypes.ValidationError, "Amount is incorrect or null");

      bool thisNewItem = await _repository.AnyAsync<EntityProduct>(e => e.Name == request.Name);
      if (thisNewItem)
        throw new CustomException(ErrorTypes.BadRequest, "This name is already in use");

      var parent = await _repository.FindSingleAsync<EntityProductCategory>(e => e.Name == request.CategoryName);
      if (parent == null)
        throw new CustomException(ErrorTypes.BadRequest, "Parent not found");

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
    #endregion

    #region SetAddOn
    public async Task<EntityAddOn> SetAddOn(AddOnRequest request)
    {
      bool isValidName = RegExHelper.ChackString(request.Name, RegExPatterns.ProductName);
      if (!isValidName)
        throw new CustomException(ErrorTypes.ValidationError, "Name is incorrect or null");

      bool isValidProductName = RegExHelper.ChackString(request.ProductName, RegExPatterns.ProductName);
      if (!isValidProductName)
        throw new CustomException(ErrorTypes.ValidationError, "Image is incorrect or null");

      if (request.Price < 0)
        throw new CustomException(ErrorTypes.ValidationError, "Price is incorrect or null");

      if (request.Amount <= 0 && request.Amount >= 10)
        throw new CustomException(ErrorTypes.ValidationError, "Amount is incorrect or null");

      var parent = await _repository.FindSingleAsync<EntityProduct>(e => e.Name == request.ProductName);
      if (parent == null)
        throw new CustomException(ErrorTypes.BadRequest, "Parent not found");

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
    #endregion

    #region RemoveProductCategories
    public async Task<IEnumerable<EntityProductCategory>> RemoveProductCategories(params string[] names)
    {
      if (names.Length <= 0)
        throw new CustomException(ErrorTypes.BadRequest, $"Objects not found");
      if (names.Length > 10)
        throw new CustomException(ErrorTypes.BadRequest, $"You can delete up to 10 objects at a time");

      foreach (var name in names)
      {
        bool isValidName = RegExHelper.ChackString(name, RegExPatterns.ProductName);
        if (!isValidName)
          throw new CustomException(ErrorTypes.ValidationError, $"Name: '{name}' is incorrect or null");
      }

      var removeItems = new List<EntityProductCategory>();
      foreach (var name in names)
      {
        var entity = await _repository.FindSingleAsync<EntityProductCategory>(e => e.Name == name);
        if (entity == null)
          throw new CustomException(ErrorTypes.BadRequest, $"Object '{name}' not found");
        removeItems.Add(entity);
      }

      await _repository.RemoveManyAsync<EntityProductCategory>(removeItems);

      return await _repository.GetEnumerable<EntityProductCategory>();
    }
    #endregion

    #region RemoveProducts
    public async Task<IEnumerable<EntityProduct>> RemoveProducts(params string[] names)
    {
      if (names.Length <= 0)
        throw new CustomException(ErrorTypes.BadRequest, $"Objects not found");
      if (names.Length > 10)
        throw new CustomException(ErrorTypes.BadRequest, $"You can delete up to 10 objects at a time");

      foreach (var name in names)
      {
        bool isValidName = RegExHelper.ChackString(name, RegExPatterns.ProductName);
        if (!isValidName)
          throw new CustomException(ErrorTypes.ValidationError, $"Name: '{name}' is incorrect or null");
      }

      var removeItems = new List<EntityProduct>();
      foreach (var name in names)
      {
        var entity = await _repository.FindSingleAsync<EntityProduct>(e => e.Name == name);
        if (entity == null)
          throw new CustomException(ErrorTypes.BadRequest, $"Object '{name}' not found");
        removeItems.Add(entity);
      }

      await _repository.RemoveManyAsync<EntityProduct>(removeItems);

      return await _repository.GetEnumerable<EntityProduct>();
    }
    #endregion

    #region RemoveAddOns
    public async Task<IEnumerable<EntityAddOn>> RemoveAddOns(params string[] names)
    {
      if (names.Length <= 0)
        throw new CustomException(ErrorTypes.BadRequest, $"Objects not found");
      if (names.Length > 10)
        throw new CustomException(ErrorTypes.BadRequest, $"You can delete up to 10 objects at a time");

      foreach (var name in names)
      {
        bool isValidName = RegExHelper.ChackString(name, RegExPatterns.ProductName);
        if (!isValidName)
          throw new CustomException(ErrorTypes.ValidationError, $"Name: '{name}' is incorrect or null");
      }

      var removeItems = new List<EntityAddOn>();
      foreach (var name in names)
      {
        var entity = await _repository.FindSingleAsync<EntityAddOn>(e => e.Name == name);
        if (entity == null)
          throw new CustomException(ErrorTypes.BadRequest, $"Object '{name}' not found");
        removeItems.Add(entity);
      }

      await _repository.RemoveManyAsync<EntityAddOn>(removeItems);

      return await _repository.GetEnumerable<EntityAddOn>();
    }
    #endregion
  }
}