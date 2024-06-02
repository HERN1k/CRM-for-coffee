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

    public async Task<T> FindSingleAsync(Expression<Func<T, bool>> predicate)
    {
      try
      {
        var entity = await _context.Set<T>()
          .Where(predicate)
          .SingleOrDefaultAsync();

        if (entity == null)
          throw new NullReferenceException();

        return entity;
      }
      catch (NullReferenceException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Null reference exception");
      }
      catch (ArgumentNullException)
      {
        throw new CustomException(ErrorTypes.ServerError, "Argument null exception");
      }
      catch (InvalidOperationException ex)
      {
        _logger.LogError(ex, "Invalid operation exception");
        throw new CustomException(ErrorTypes.ServerError, "Cannot be executed due to the current state of the object or data context", ex);
      }
      catch (OperationCanceledException ex)
      {
        _logger.LogError(ex, "Operation canceled exception");
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unexpected exception occurred");
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }

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
        _logger.LogError(ex, "Operation canceled exception");
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unexpected exception occurred");
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }

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
        _logger.LogError(ex, "Operation canceled exception");
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unexpected exception occurred");
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }

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
        _logger.LogError(ex, "Operation canceled exception");
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unexpected exception occurred");
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }

    public async Task AddAsync(T entity)
    {
      try
      {
        if (entity == null)
          throw new ArgumentNullException(string.Empty);

        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
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
        _logger.LogError(ex, "Database update concurrency exception");
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, "Database update exception");
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (OperationCanceledException ex)
      {
        _logger.LogError(ex, "Operation canceled exception");
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unexpected exception occurred");
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }

    public async Task UpdateAsync(T entity)
    {
      try
      {
        if (entity == null)
          throw new ArgumentNullException(string.Empty);

        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
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
        _logger.LogError(ex, "Database update concurrency exception");
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, "Database update exception");
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (OperationCanceledException ex)
      {
        _logger.LogError(ex, "Operation canceled exception");
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unexpected exception occurred");
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }

    public async Task RemoveAsync(T entity)
    {
      try
      {
        if (entity == null)
          throw new ArgumentNullException(string.Empty);

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
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
        _logger.LogError(ex, "Database update concurrency exception");
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, "Database update exception");
        throw new CustomException(ErrorTypes.ServerError, "Database update exception", ex);
      }
      catch (OperationCanceledException ex)
      {
        _logger.LogError(ex, "Operation canceled exception");
        throw new CustomException(ErrorTypes.ServerError, "The operation was canceled externally", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unexpected exception occurred");
        throw new CustomException(ErrorTypes.ServerError, "An unexpected database exception occurred", ex);
      }
    }
  }
}