using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Data.Repositories
{
  public class Repository<T> : IRepository<T> where T : class
  {
    protected readonly ILogger<Repository<T>> _logger;
    protected readonly AppDBContext _context;

    public Repository(
        ILogger<Repository<T>> logger,
        AppDBContext context
      )
    {
      _logger = logger;
      _context = context;
    }

    #region FindSingleAsync
    public async Task<T> FindSingleAsync(Expression<Func<T, bool>> predicate)
    {
      try
      {
        var entity = await _context.Set<T>()
          .Where(predicate)
          .SingleOrDefaultAsync();

        if (entity == null)
          throw new InvalidOperationException("Entity not found");

        return entity;
      }
      catch (InvalidOperationException ex)
      {
        throw new CustomException(ErrorTypes.InvalidOperationException, ex.Message);
      }
      catch (NullReferenceException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Null reference exception");
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

    #region AnyAsync
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
      try
      {
        return await _context.Set<T>()
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
    public async Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> predicate)
    {
      try
      {
        return await _context.Set<T>()
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

    #region GetAllAsync
    public async Task<IEnumerable<T>> GetAllAsync()
    {
      try
      {
        return await _context.Set<T>()
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
    public async Task AddAsync(T entity)
    {
      try
      {
        if (entity == null)
          throw new InvalidOperationException("Entity not found");

        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
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
    public async Task UpdateAsync(T entity)
    {
      try
      {
        if (entity == null)
          throw new InvalidOperationException();

        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
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
    public async Task RemoveAsync(T entity)
    {
      try
      {
        if (entity == null)
          throw new InvalidOperationException();

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
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
  }
}