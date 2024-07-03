using System.Linq.Expressions;
using CRM.Core.Exceptions.Custom;

namespace CRM.Core.Interfaces.Repositories.Base
{
    /// <summary>
    ///   Provides base repository methods for data access.
    /// </summary>
    public interface IBaseRepository
  {
    /// <summary>
    ///   Finds a single entity that matches the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="predicate">The predicate to match.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the found entity or null.</returns>
    /// <exception cref="CustomException">Thrown if an unexpected error occurs during the operation.</exception>
    Task<T?> FindSingleAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

    /// <summary>
    ///   Checks if any entities match the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="predicate">The predicate to match.</param>
    /// <returns>
    ///   A task representing the asynchronous operation. 
    ///   The task result contains a boolean indicating whether any entities match the predicate.
    /// </returns>
    /// <exception cref="CustomException">Thrown if an unexpected error occurs during the operation.</exception>
    Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

    /// <summary>
    ///   Gets an <see cref="IQueryable{T}"/> for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <returns>An <see cref="IQueryable{T}"/> for the specified entity type.</returns>
    IQueryable<T> GetQueryable<T>() where T : class;

    /// <summary>
    ///   Gets all entities of the specified type as an enumerable.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <returns>A task representing the asynchronous operation. The task result contains the list of entities.</returns>
    /// <exception cref="CustomException">Thrown if an unexpected error occurs during the operation.</exception>
    Task<IEnumerable<T>> GetEnumerable<T>() where T : class;

    /// <summary>
    ///   Adds a new entity to the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="CustomException">Thrown if an unexpected error occurs during the operation.</exception>
    Task AddAsync<T>(T entity) where T : class;

    /// <summary>
    ///   Updates an existing entity in the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="CustomException">Thrown if an unexpected error occurs during the operation.</exception>
    Task UpdateAsync<T>(T entity) where T : class;

    /// <summary>
    ///   Removes entities that match the specified predicate from the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="predicate">The predicate to match.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="CustomException">Thrown if an unexpected error occurs during the operation.</exception>
    Task RemoveAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

    /// <summary>
    ///   Removes multiple entities from the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entities">The entities to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="CustomException">Thrown if an unexpected error occurs during the operation.</exception>
    Task RemoveManyAsync<T>(List<T> entities) where T : class;

    /// <summary>
    ///   Disposes the context asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask DisposeAsync();
  }
}