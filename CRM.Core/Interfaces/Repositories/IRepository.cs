using System.Linq.Expressions;

namespace CRM.Core.Interfaces.Repositories
{
  public interface IRepository<T> where T : class
  {
    Task<T?> FindSingleAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> predicate);
    IQueryable<T> GetQueryable();
    Task<IEnumerable<T>> GetEnumerable();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
  }
}