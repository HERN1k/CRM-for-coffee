using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Models;
using CRM.Core.Responses;

namespace CRM.Core.Interfaces.Services.BLogicServices.OrderServices
{
  /// <summary>
  ///   Provides services for managing orders.
  /// </summary>
  public interface IOrderService
  {
    /// <summary>
    ///   Adds a new order asynchronously.
    /// </summary>
    /// <param name="input">The create order request.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the operation result.</returns>
    Task<OperationResult> AddOrderAsync(CreateOrderRequest input);

    /// <summary>
    ///   Gets the orders for the current employee for the current day asynchronously.
    /// </summary>
    /// <returns>
    ///   A task representing the asynchronous operation. The task result contains the collection of orders for the current day.
    /// </returns>
    Task<IEnumerable<Order>> GetEmployeeOrdersForDayAsync();

    /// <summary>
    ///   Gets a queryable collection of all orders.
    /// </summary>
    /// <returns>A queryable collection of all orders.</returns>
    IQueryable<Order> GetOrders();
  }
}