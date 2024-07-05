using System.Linq.Expressions;

using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Entity;
using CRM.Core.Models;

namespace CRM.Core.Interfaces.Repositories.Excel
{
  /// <summary>
  ///   Provides methods for retrieving and manipulating data related to Excel reports from the repository.
  /// </summary>
  public interface IExcelRepository
  {
    /// <summary>
    /// Asynchronously retrieves a collection of entities of the specified type.
    /// </summary>
    /// <typeparam name="TModel">The type of the model to map to.</typeparam>
    /// <typeparam name="TEntity">The type of the entity to find in the repository.</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/>
    ///   collection of the specified entities.
    /// </returns>
    /// <exception cref="CustomException"></exception>
    Task<List<TModel>> FindEntities<TModel, TEntity>() where TModel : class where TEntity : class;

    /// <summary>
    /// Asynchronously retrieves an entity of the specified type by its identifier.
    /// </summary>
    /// <typeparam name="TModel">The type of the model to map to. Must implement <see cref="IEntityWithId"/>.</typeparam>
    /// <typeparam name="TEntity">The type of the entity to find in the repository. Must implement <see cref="IEntityWithId"/>.</typeparam>
    /// <param name="id">The ID of the entity to find.</param>
    /// <returns>A model of type <typeparamref name="TModel"/>.</returns>
    /// <exception cref="CustomException">Thrown when the entity is not found or there is a server error.</exception>
    Task<TModel> FindEntityById<TModel, TEntity>(Guid id) where TModel : class, IEntityWithId where TEntity : class, IEntityWithId;

    /// <summary>
    ///   Counts the number of entities of the specified type that satisfy the given predicate.
    /// </summary>
    /// <typeparam name="T">The type of the entities to count. Must be a class.</typeparam>
    /// <param name="predicate">The predicate to apply to the entities.</param>
    /// <returns>The number of entities that satisfy the predicate.</returns>
    /// <exception cref="CustomException"></exception>
    int NumberEntitiesWithPredicate<T>(Expression<Func<T, bool>> predicate) where T : class;

    /// <summary>
    ///   Retrieves a queryable collection of product category entities, including their related products.
    /// </summary>
    /// <returns>
    ///   An <see cref="List{ProductCategory}"/> collection of product category entities.
    /// </returns>
    /// <exception cref="CustomException"></exception>
    List<ProductCategory> FindEntitiesProductCategory();

    /// <summary>
    ///   Retrieves a queryable collection of order entities, including their related products and addons.
    /// </summary>
    /// <returns>
    ///   An <see cref="List{Order}"/> collection of orders.
    /// </returns>
    /// <exception cref="CustomException"></exception>
    List<Order> FindEntitiesOrder();

    /// <summary>
    ///   Retrieves a queryable collection of order entities within the specified date range, including their related products and addons.
    /// </summary>
    /// <param name="startDate">The start date of the date range for which to retrieve orders.</param>
    /// <param name="endDate">The end date of the date range for which to retrieve orders.</param>
    /// <returns>
    ///   An <see cref="List{Order}"/> collection of orders within the specified date range.
    /// </returns>
    /// <exception cref="CustomException"></exception>
    List<Order> FindEntitiesOrderByDate(DateTime startDate, DateTime endDate);
  }
}