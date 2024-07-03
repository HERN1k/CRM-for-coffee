using System.Linq.Expressions;

using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Entity;
using CRM.Core.Interfaces.Repositories.Base;
using CRM.Core.Interfaces.Repositories.Excel;

using Microsoft.EntityFrameworkCore;

namespace CRM.Data.Repositories.Excel
{
  public class ExcelRepository(
      IBaseRepository repository
    ) : IExcelRepository
  {
    private readonly IBaseRepository _repository = repository;

    public async Task<IEnumerable<T>> FindEntities<T>() where T : class
    {
      var result = await _repository
        .GetEnumerable<T>()
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");
      return result;
    }

    public async Task<T> FindEntityById<T>(Guid id) where T : class, IEntityWithId
    {
      var result = await _repository
        .FindSingleAsync<T>(e => e.Id == id)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");
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

    public IQueryable<EntityProductCategory> FindEntitiesProductCategory()
    {
      var result = _repository
        .GetQueryable<EntityProductCategory>()
        .AsNoTracking()
        .Include(e => e.Products)
        .OrderBy(e => e.Name)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");
      return result;
    }

    public IQueryable<EntityOrder> FindEntitiesOrder()
    {
      var result = _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Include(e => e.Products)
          .ThenInclude(e => e.AddOns)
        .OrderBy(e => e.OrderСreationDate)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");
      return result;
    }

    public IQueryable<EntityOrder> FindEntitiesOrderByDate(DateTime startDate, DateTime endDate)
    {
      var result = _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Where(e => e.OrderСreationDate > startDate.ToUniversalTime())
        .Where(e => e.OrderСreationDate < endDate.ToUniversalTime())
        .Include(e => e.Products)
          .ThenInclude(e => e.AddOns)
        .OrderBy(e => e.OrderСreationDate)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");
      return result;
    }
  }
}