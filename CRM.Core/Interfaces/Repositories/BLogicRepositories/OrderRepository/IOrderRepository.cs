using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Entity;
using CRM.Core.Models;

namespace CRM.Core.Interfaces.Repositories.BLogicRepositories.OrderRepository
{
  /// <summary>
  ///   Provides repository methods for managing orders.
  /// </summary>
  public interface IOrderRepository
  {
    /// <summary>
    ///   Finds a worker by email.
    /// </summary>
    /// <param name="email">The email of the worker to find.</param>
    /// <param name="type">The error type to throw if the worker is not found.</param>
    /// <param name="message">The error message to throw if the worker is not found.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the found <see cref="User"/>.</returns>
    /// <exception cref="CustomException">Thrown if the worker is not found.</exception>
    Task<User> FindWorkerByEmail(string email, ErrorTypes type, string message);

    /// <summary>
    ///   Finds an entity by ID.
    /// </summary>
    /// <typeparam name="TModel">The type of the model to map to. Must implement <see cref="IEntityWithId"/>.</typeparam>
    /// <typeparam name="TEntity">The type of the entity to find in the repository. Must implement <see cref="IEntityWithId"/>.</typeparam>
    /// <param name="id">The ID of the entity to find.</param>
    /// <param name="type">The error type to throw if the entity is not found.</param>
    /// <param name="message">The error message to throw if the entity is not found.</param>
    /// <returns>A model of type <typeparamref name="TModel"/>.</returns>
    /// <exception cref="CustomException">Thrown if the entity is not found.</exception>
    Task<TModel> FindEntityById<TModel, TEntity>(Guid id, ErrorTypes type, string message) where TModel : class, IEntityWithId where TEntity : class, IEntityWithId;

    /// <summary>
    ///   Gets the orders for a specific worker for the current day, ordered by descending order creation date.
    /// </summary>
    /// <param name="user">The worker for whom to get the orders.</param>
    /// <returns>A collection of orders.</returns>
    List<Order> GetOrdersByDescendingWorkerPerDay(User user);

    /// <summary>
    ///   Gets the number of orders for a specific worker for the current day.
    /// </summary>
    /// <param name="user">The worker for whom to get the number of orders.</param>
    /// <returns>The number of orders.</returns>
    int GetNumberOrders(User user);

    /// <summary>
    ///   Gets a collection of all orders.
    /// </summary>
    /// <returns>A collection of all orders.</returns>
    IQueryable<Order> GetQueryableOrders();

    /// <summary>
    ///   Adds a new order to the database asynchronously.
    /// </summary>
    /// <param name="order">The order to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddOrderAsync(Order order);
  }
}