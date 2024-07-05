using CRM.Application.Tools.RequestValidation;
using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Enums;
using CRM.Core.Interfaces.Repositories.BLogicRepositories.OrderRepository;
using CRM.Core.Interfaces.Services.BLogicServices.OrderServices;
using CRM.Core.Models;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Http;

namespace CRM.Application.Services.OrderServices
{
  public class OrderService(
      IOrderComponents components,
      IOrderRepository repository
    ) : IOrderService
  {
    private readonly IOrderComponents _components = components;
    private readonly IOrderRepository _repository = repository;

    public async Task<OperationResult> AddOrderAsync(CreateOrderRequest input)
    {
      RequestValidator.Validate(input);

      CreateOrderDto newOrder = _components.CreateOrderDataMapper(input);

      User workerChackout = await _repository
        .FindWorkerByEmail(input.WorkerEmail, ErrorTypes.BadRequest, "Worker not found");

      newOrder.WorkerId = workerChackout.Id;

      var products = await _components.GetOrderProductList(input, newOrder);

      newOrder.Products = products;

      Order order = new Order(newOrder);

      await _repository.AddOrderAsync(order);

      return new OperationResult(true);
    }

    public async Task<IEnumerable<Order>> GetEmployeeOrdersForDayAsync()
    {
      HttpContext httpContext = _components.GetHttpContext();

      string email = _components.GetEmailFromUserClaims(httpContext);

      User user = await _repository
        .FindWorkerByEmail(email, ErrorTypes.ServerError, "Server error");

      List<Order> orders = _repository
        .GetOrdersByDescendingWorkerPerDay(user);

      int numberOrders = _repository.GetNumberOrders(user);

      _components.SetOrderNumber(orders, numberOrders);

      return orders.AsEnumerable();
    }

    public IQueryable<Order> GetOrders() =>
      _repository.GetQueryableOrders();
  }
}