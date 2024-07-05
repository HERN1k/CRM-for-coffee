using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;

namespace CRM.Core.Interfaces.Services.BLogicServices.OrderServices
{
  /// <summary>
  ///   Provides helper methods for managing orders.
  /// </summary>
  public interface IOrderComponents
  {
    /// <summary>
    ///   Maps the request data to the create order data.
    /// </summary>
    /// <param name="request">The create order request.</param>
    /// <returns>The mapped create order data.</returns>
    CreateOrderDto CreateOrderDataMapper(CreateOrderRequest request);

    /// <summary>
    ///   Gets the list of order products based on the request and new order.
    /// </summary>
    /// <param name="request">The create order request.</param>
    /// <param name="newOrder">The new order.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the list of order products.</returns>
    Task<List<OrderProduct>> GetOrderProductList(CreateOrderRequest request, CreateOrderDto newOrder);

    /// <summary>
    ///   Gets the order product based on the provided product and new order.
    /// </summary>
    /// <param name="product">The product.</param>
    /// <param name="newOrder">The new order.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the order product.</returns>
    Task<OrderProduct> GetOrderProduct(OrderProduct product, CreateOrderDto newOrder);

    /// <summary>
    ///   Gets the order add-on based on the provided add-on, temporary product, and new order.
    /// </summary>
    /// <param name="addOn">The add-on.</param>
    /// <param name="tempProduct">The temporary product.</param>
    /// <param name="newOrder">The new order.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task GetOrderAddOn(OrderAddOn addOn, OrderProduct tempProduct, CreateOrderDto newOrder);

    /// <summary>
    ///   Gets the current HTTP context.
    /// </summary>
    /// <returns>The current HTTP context.</returns>
    /// <exception cref="CustomException">Thrown if the HTTP context is null.</exception>
    HttpContext GetHttpContext();

    /// <summary>
    ///   Gets the email from the user claims in the provided HTTP context.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>The email from the user claims.</returns>
    /// <exception cref="CustomException">Thrown if the email is not found in the user claims.</exception>
    string GetEmailFromUserClaims(HttpContext httpContext);

    /// <summary>
    ///   Sets the order number for the list of orders.
    /// </summary>
    /// <param name="orders">The collection of orders.</param>
    /// <param name="numberOrders">The number of orders.</param>
    void SetOrderNumber(List<Order> orders, int numberOrders);
  }
}