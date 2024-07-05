using System.Linq.Expressions;

using AutoMapper;

using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Entity;
using CRM.Core.Interfaces.Repositories.Base;
using CRM.Core.Interfaces.Repositories.Excel;
using CRM.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.Repositories.Excel
{
  public class ExcelRepository(
      IBaseRepository repository,
      IMapper mapper
    ) : IExcelRepository
  {
    private readonly IBaseRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<List<TModel>> FindEntities<TModel, TEntity>() where TModel : class where TEntity : class
    {
      var entities = await _repository
        .GetEnumerable<TEntity>()
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      var result = new List<TModel>();

      foreach (var entity in entities)
      {
        var temp = _mapper.Map<TModel>(entity);
        result.Add(temp);
      }

      return result;
    }

    public async Task<TModel> FindEntityById<TModel, TEntity>(Guid id) where TModel : class, IEntityWithId where TEntity : class, IEntityWithId
    {
      var entity = await _repository
        .FindSingleAsync<TEntity>(e => e.Id == id)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      var result = _mapper.Map<TModel>(entity);

      return result;
    }

    public int NumberEntitiesWithPredicate<T>(Expression<Func<T, bool>> predicate) where T : class
    {
      int result = _repository
        .GetQueryable<T>()
        .AsNoTracking()
        .Where(predicate)
        .Count();
      return result;
    }

    public List<ProductCategory> FindEntitiesProductCategory()
    {
      var entities = _repository
        .GetQueryable<EntityProductCategory>()
        .AsNoTracking()
        .Include(e => e.Products)
        .OrderBy(e => e.Name)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      var result = new List<ProductCategory>();

      foreach (var entity in entities)
      {
        ProductCategory temp = _mapper.Map<ProductCategory>(entity);
        result.Add(temp);
      }

      return result;
    }

    public List<Order> FindEntitiesOrder()
    {
      var entities = _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Include(e => e.Products)
          .ThenInclude(e => e.AddOns)
        .OrderBy(e => e.OrderСreationDate)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      var result = new List<Order>();

      foreach (var entity in entities)
      {
        Order temp = _mapper.Map<Order>(entity);
        result.Add(temp);
      }

      return result;
    }

    public List<Order> FindEntitiesOrderByDate(DateTime startDate, DateTime endDate)
    {
      var entities = _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Where(e => e.OrderСreationDate > startDate.ToUniversalTime())
        .Where(e => e.OrderСreationDate < endDate.ToUniversalTime())
        .Include(e => e.Products)
          .ThenInclude(e => e.AddOns)
        .OrderBy(e => e.OrderСreationDate)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      var result = new List<Order>();

      foreach (var entity in entities)
      {
        Order temp = _mapper.Map<Order>(entity);
        result.Add(temp);
      }

      return result;
    }
  }
}