using System.Linq.Expressions;

namespace CRM.Core.Interfaces.Repositories
{
  public interface IRepository
  {
    Task<T?> FindSingleAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
    Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
    Task<IEnumerable<T>> FindManyAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
    IQueryable<T> GetQueryable<T>() where T : class;
    Task<IEnumerable<T>> GetEnumerable<T>() where T : class;
    Task AddAsync<T>(T entity) where T : class;
    Task UpdateAsync<T>(T entity) where T : class;
    Task RemoveAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
    Task RemoveManyAsync<T>(List<T> entities) where T : class;
  }
}