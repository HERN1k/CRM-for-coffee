using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Data.Repositories
{
  public class Repository : IAsyncDisposable, IRepository
  {
    private readonly ILogger<Repository> _logger;
    private readonly AppDBContext _сontext;

    public Repository(
        ILogger<Repository> logger,
        IDbContextFactory<AppDBContext> contextFactory
      )
    {
      _logger = logger;
      _сontext = contextFactory.CreateDbContext();
    }

    #region FindSingleAsync
    public async Task<T?> FindSingleAsync<T>(
        Expression<Func<T, bool>> predicate
      ) where T : class
    {
      try
      {
        return await _сontext.Set<T>()
          .Where(predicate)
          .SingleOrDefaultAsync();
      }
      catch (ArgumentNullException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Argument null exception");
      }
      catch (InvalidOperationException ex)
      {
        throw new CustomException(ErrorTypes.InvalidOperationException, ex.Message);
      }
      catch (OperationCanceledException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }
    #endregion

    #region AnyAsync
    public async Task<bool> AnyAsync<T>(
        Expression<Func<T, bool>> predicate
      ) where T : class
    {
      try
      {
        return await _сontext.Set<T>()
          .AsNoTracking()
          .Where(predicate)
          .AnyAsync();
      }
      catch (ArgumentNullException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Argument null exception");
      }
      catch (OperationCanceledException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }
    #endregion

    #region FindManyAsync
    public async Task<IEnumerable<T>> FindManyAsync<T>(
        Expression<Func<T, bool>> predicate
      ) where T : class
    {
      try
      {
        return await _сontext.Set<T>()
          .Where(predicate)
          .ToListAsync();
      }
      catch (ArgumentNullException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Argument null exception");
      }
      catch (OperationCanceledException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }
    #endregion

    #region GetQueryable
    public IQueryable<T> GetQueryable<T>() where T : class
    {
      return _сontext.Set<T>();
    }
    #endregion

    #region GetEnumerable
    public async Task<IEnumerable<T>> GetEnumerable<T>() where T : class
    {
      try
      {
        return await _сontext.Set<T>()
          .ToListAsync();
      }
      catch (ArgumentNullException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Argument null exception");
      }
      catch (OperationCanceledException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }
    #endregion

    #region AddAsync
    public async Task AddAsync<T>(T entity) where T : class
    {
      try
      {
        if (entity == null)
          throw new InvalidOperationException("Entity not found");

        await _сontext.Set<T>().AddAsync(entity);
        await _сontext.SaveChangesAsync();
      }
      catch (InvalidOperationException ex)
      {
        throw new CustomException(ErrorTypes.InvalidOperationException, ex.Message);
      }
      catch (ArgumentNullException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Argument null exception");
      }
      catch (ValidationException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Database validation exception");
      }
      catch (DbUpdateConcurrencyException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (OperationCanceledException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }
    #endregion

    #region UpdateAsync
    public async Task UpdateAsync<T>(T entity) where T : class
    {
      try
      {
        if (entity == null)
          throw new InvalidOperationException("Entity not found");

        _сontext.Set<T>().Update(entity);
        await _сontext.SaveChangesAsync();
      }
      catch (InvalidOperationException ex)
      {
        throw new CustomException(ErrorTypes.InvalidOperationException, ex.Message);
      }
      catch (ArgumentNullException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Argument null exception");
      }
      catch (ValidationException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Database validation exception");
      }
      catch (DbUpdateConcurrencyException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (OperationCanceledException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }
    #endregion

    #region RemoveAsync
    public async Task RemoveAsync<T>(Expression<Func<T, bool>> predicate) where T : class
    {
      try
      {
        _сontext.Set<T>()
          .RemoveRange(_сontext.Set<T>().Where(predicate));

        await _сontext.SaveChangesAsync();
      }
      catch (ArgumentNullException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Argument null exception");
      }
      catch (ValidationException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Database validation exception");
      }
      catch (DbUpdateConcurrencyException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (OperationCanceledException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }
    #endregion

    #region RemoveManyAsync
    public async Task RemoveManyAsync<T>(List<T> entities) where T : class
    {
      try
      {
        if (entities == null || entities.Count <= 0)
          throw new InvalidOperationException("Entity not found");

        _сontext.Set<T>().RemoveRange(entities);
        await _сontext.SaveChangesAsync();
      }
      catch (InvalidOperationException ex)
      {
        throw new CustomException(ErrorTypes.InvalidOperationException, ex.Message);
      }
      catch (ArgumentNullException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Argument null exception");
      }
      catch (ValidationException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Database validation exception");
      }
      catch (DbUpdateConcurrencyException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (OperationCanceledException ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }
    #endregion

    #region DisposeAsync
    public ValueTask DisposeAsync()
    {
      return _сontext.DisposeAsync();
    }
    #endregion
  }
}