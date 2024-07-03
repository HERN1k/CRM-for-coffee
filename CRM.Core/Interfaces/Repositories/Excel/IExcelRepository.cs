using System.Linq.Expressions;
using CRM.Core.Entities;
using CRM.Core.Interfaces.Entity;

namespace CRM.Core.Interfaces.Repositories.Excel
{
    /// <summary>
    ///   Provides methods for retrieving and manipulating data related to Excel reports from the repository.
    /// </summary>
    public interface IExcelRepository
    {
        /// <summary>
        ///   Asynchronously retrieves a collection of entities of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the entities to retrieve. Must be a class.</typeparam>
        /// <returns>
        ///   A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/>
        ///   collection of the specified entities.</returns>
        /// <exception cref="CustomException"></exception>
        Task<IEnumerable<T>> FindEntities<T>() where T : class;

        /// <summary>
        ///   Asynchronously retrieves an entity of the specified type by its identifier.
        /// </summary>
        /// <typeparam name="T">The type of the entity to retrieve. Must be a class and implement <see cref="IEntityWithId"/>.</typeparam>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>
        ///   A task that represents the asynchronous operation. 
        ///   The task result contains an instance of <typeparamref name="T"/> representing the retrieved entity.
        /// </returns>
        /// <exception cref="CustomException"></exception>
        Task<T> FindEntityById<T>(Guid id) where T : class, IEntityWithId;

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
        ///   An <see cref="IQueryable{EntityProductCategory}"/> collection of product category entities.
        /// </returns>
        /// <exception cref="CustomException"></exception>
        IQueryable<EntityProductCategory> FindEntitiesProductCategory();

        /// <summary>
        ///   Retrieves a queryable collection of order entities, including their related products and addons.
        /// </summary>
        /// <returns>
        ///   An <see cref="IQueryable{EntityOrder}"/> collection of order entities.
        /// </returns>
        /// <exception cref="CustomException"></exception>
        IQueryable<EntityOrder> FindEntitiesOrder();

        /// <summary>
        ///   Retrieves a queryable collection of order entities within the specified date range, including their related products and addons.
        /// </summary>
        /// <param name="startDate">The start date of the date range for which to retrieve orders.</param>
        /// <param name="endDate">The end date of the date range for which to retrieve orders.</param>
        /// <returns>
        ///   An <see cref="IQueryable{EntityOrder}"/> collection of order entities within the specified date range.
        /// </returns>
        /// <exception cref="CustomException"></exception>
        IQueryable<EntityOrder> FindEntitiesOrderByDate(DateTime startDate, DateTime endDate);
    }
}