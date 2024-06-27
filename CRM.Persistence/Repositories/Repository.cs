using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Data.Repositories
{
  public class Repository(
      ILogger<Repository> logger,
      IDbContextFactory<AppDBContext> contextFactory
    ) : IAsyncDisposable, IRepository
  {
    private readonly ILogger<Repository> _logger = logger;
    private readonly AppDBContext _сontext = contextFactory.CreateDbContext();

    /// <summary>
    ///   Finds a single entity asynchronously that matches the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="predicate">The predicate to filter the entity.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation. The task result contains the entity that matches the predicate, 
    ///   or null if no such entity is found.
    /// </returns>
    /// <exception cref="CustomException">
    ///   Thrown when an error occurs during the operation:
    /// <list type="bullet">
    /// <item><description><see cref="ErrorTypes.ServerError"/>: If an argument is null or an unexpected exception occurs.</description></item>
    /// <item><description><see cref="ErrorTypes.InvalidOperationException"/>: If the query returns more than one entity.</description></item>
    /// </list>
    /// </exception>
    public async Task<T?> FindSingleAsync<T>(Expression<Func<T, bool>> predicate) where T : class
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

    /// <summary>
    ///   Determines asynchronously whether any elements in the set match the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation. The task result contains true if any elements match the predicate; otherwise, false.
    /// </returns>
    /// <exception cref="CustomException">
    ///   Thrown when an error occurs during the operation:
    /// <list type="bullet">
    /// <item><description><see cref="ErrorTypes.ServerError"/>: If an argument is null or an unexpected exception occurs.</description></item>
    /// </list>
    /// </exception>
    public async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class
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

    /// <summary>
    ///   Finds multiple entities asynchronously that match the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation. The task result contains a collection of entities that match the predicate.
    /// </returns>
    /// <exception cref="CustomException">
    ///   Thrown when an error occurs during the operation:
    /// <list type="bullet">
    /// <item><description><see cref="ErrorTypes.ServerError"/>: If an argument is null or an unexpected exception occurs.</description></item>
    /// </list>
    /// </exception>
    public async Task<IEnumerable<T>> FindManyAsync<T>(Expression<Func<T, bool>> predicate) where T : class
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

    /// <summary>
    ///   Gets an <see cref="IQueryable{T}"/> for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <returns>An <see cref="IQueryable{T}"/> for the specified entity type.</returns>
    public IQueryable<T> GetQueryable<T>() where T : class
    {
      return _сontext.Set<T>();
    }

    /// <summary>
    ///   Retrieves all entities of the specified type from the database asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <returns>
    ///   A task that represents the asynchronous operation. The task result contains an enumerable collection of entities of the specified type.
    /// </returns>
    /// <exception cref="CustomException">
    ///   Thrown when an error occurs during the operation:
    /// <list type="bullet">
    /// <item><description><see cref="ErrorTypes.ServerError"/>: If an argument is null or an unexpected exception occurs.</description></item>
    /// </list>
    /// </exception>
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

    /// <summary>
    ///   Adds a new entity to the database asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entity">The entity to add to the database.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="CustomException">
    ///   Thrown when an error occurs during the operation:
    /// <list type="bullet">
    /// <item><description><see cref="ErrorTypes.InvalidOperationException"/>: If the entity is null.</description></item>
    /// <item><description><see cref="ErrorTypes.ServerError"/>: If an argument is null, a validation exception occurs, or an unexpected exception occurs.</description></item>
    /// </list>
    /// </exception>
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

    /// <summary>
    ///   Updates an existing entity in the database asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entity">The entity to update in the database.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="CustomException">
    ///   Thrown when an error occurs during the operation:
    /// <list type="bullet">
    /// <item><description><see cref="ErrorTypes.InvalidOperationException"/>: If the entity is null.</description></item>
    /// <item><description><see cref="ErrorTypes.ServerError"/>: If an argument is null, a validation exception occurs, or an unexpected exception occurs.</description></item>
    /// </list>
    /// </exception>
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

    /// <summary>
    ///   Removes entities from the database asynchronously that match the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="predicate">The predicate to filter the entities to be removed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="CustomException">
    ///   Thrown when an error occurs during the operation:
    /// <list type="bullet">
    /// <item><description><see cref="ErrorTypes.ServerError"/>: If an argument is null, a validation exception occurs, or an unexpected exception occurs.</description></item>
    /// </list>
    /// </exception>
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

    /// <summary>
    ///   Removes a list of entities from the database asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entities">The list of entities to remove from the database.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="CustomException">
    ///   Thrown when an error occurs during the operation:
    /// <list type="bullet">
    /// <item><description><see cref="ErrorTypes.InvalidOperationException"/>: If the list of entities is null or empty.</description></item>
    /// <item><description><see cref="ErrorTypes.ServerError"/>: If an argument is null, a validation exception occurs, or an unexpected exception occurs.</description></item>
    /// </list>
    /// </exception>
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

    public ValueTask DisposeAsync()
    {
      return _сontext.DisposeAsync();
    }
  }
}