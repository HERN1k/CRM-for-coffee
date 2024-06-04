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
    private readonly IRepository<EntityProductCategory> _productCategoryRepository;
    private readonly IRepository<EntityProduct> _productRepository;
    private readonly IRepository<EntityAddOn> _addOnRepository;

    public ProductsService(
        IRepository<EntityProductCategory> productCategoryRepository,
        IRepository<EntityProduct> productRepository,
        IRepository<EntityAddOn> addOnRepository
      )
    {
      _productCategoryRepository = productCategoryRepository;
      _productRepository = productRepository;
      _addOnRepository = addOnRepository;
    }

    #region GetProductCategories
    public IQueryable<EntityProductCategory> GetProductCategories() =>
      _productCategoryRepository.GetQueryable();
    #endregion

    #region GetProducts
    public IQueryable<EntityProduct> GetProducts() =>
      _productRepository.GetQueryable();
    #endregion

    #region GetAddOns
    public IQueryable<EntityAddOn> GetAddOns() =>
      _addOnRepository.GetQueryable();
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

      bool thisNewItem = await _productCategoryRepository.AnyAsync(e => e.Name == request.Name);
      if (thisNewItem)
        throw new CustomException(ErrorTypes.BadRequest, "This name is already in use");

      var newProductCategory = new EntityProductCategory
      {
        Image = request.Image,
        Name = request.Name
      };

      await _productCategoryRepository.AddAsync(newProductCategory);

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

      bool thisNewItem = await _productRepository.AnyAsync(e => e.Name == request.Name);
      if (thisNewItem)
        throw new CustomException(ErrorTypes.BadRequest, "This name is already in use");

      var parent = await _productCategoryRepository.FindSingleAsync(e => e.Name == request.CategoryName);
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

      await _productRepository.AddAsync(newProduct);

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

      if (request.Amount < 1)
        throw new CustomException(ErrorTypes.ValidationError, "Amount is incorrect or null");

      bool thisNewItem = await _addOnRepository.AnyAsync(e => e.Name == request.Name);
      if (thisNewItem)
        throw new CustomException(ErrorTypes.BadRequest, "This name is already in use");

      var parent = await _productRepository.FindSingleAsync(e => e.Name == request.ProductName);
      if (parent == null)
        throw new CustomException(ErrorTypes.BadRequest, "Parent not found");

      var newAddOn = new EntityAddOn
      {
        ProductId = parent.Id,
        Name = request.Name,
        Price = request.Price,
        Amount = request.Amount
      };

      await _addOnRepository.AddAsync(newAddOn);

      return newAddOn;
    }
    #endregion

    #region RemoveProductCategory
    public async Task<IEnumerable<EntityProductCategory>> RemoveProductCategory(string name)
    {
      bool isValidName = RegExHelper.ChackString(name, RegExPatterns.ProductName);
      if (!isValidName)
        throw new CustomException(ErrorTypes.ValidationError, "Name is incorrect or null");

      var entity = await _productCategoryRepository.FindSingleAsync(e => e.Name == name);

      if (entity != null)
        await _productCategoryRepository.RemoveAsync(entity);

      return await _productCategoryRepository.GetEnumerable();
    }
    #endregion

    #region RemoveProduct
    public async Task<IEnumerable<EntityProduct>> RemoveProduct(string name)
    {
      bool isValidName = RegExHelper.ChackString(name, RegExPatterns.ProductName);
      if (!isValidName)
        throw new CustomException(ErrorTypes.ValidationError, "Name is incorrect or null");

      var entity = await _productRepository.FindSingleAsync(e => e.Name == name);

      if (entity != null)
        await _productRepository.RemoveAsync(entity);

      return await _productRepository.GetEnumerable();
    }
    #endregion

    #region RemoveAddOn
    public async Task<IEnumerable<EntityAddOn>> RemoveAddOn(string name)
    {
      bool isValidName = RegExHelper.ChackString(name, RegExPatterns.ProductName);
      if (!isValidName)
        throw new CustomException(ErrorTypes.ValidationError, "Name is incorrect or null");

      var entity = await _addOnRepository.FindSingleAsync(e => e.Name == name);

      if (entity != null)
        await _addOnRepository.RemoveAsync(entity);

      return await _addOnRepository.GetEnumerable();
    }
    #endregion

    #region RemoveManyProductCategories
    // !101
    #endregion

    #region RemoveManyProducts
    // !101
    #endregion

    #region RemoveManyAddOns
    // !101
    #endregion
  }
}