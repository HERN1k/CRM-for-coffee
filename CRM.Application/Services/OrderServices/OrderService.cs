using CRM.Application.RequestDataMapper;
using CRM.Application.RequestValidation;
using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Interfaces.OrderServices;
using CRM.Core.Interfaces.Repositories;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services.OrderServices
{
  public partial class OrderService(
      IHttpContextAccessor httpContextAccessor,
      IOptions<BusinessInformationSettings> businessInfSettings,
      IRepository repository
    ) : IOrderService
  {
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly BusinessInformationSettings _businessInfSettings = businessInfSettings.Value;
    private readonly IRepository _repository = repository;

    public async Task<OperationResult> AddOrderAsync(CreateOrderRequest input)
    {
      RequestValidator.Validate(input);

      CreateOrder newOrder = CreateOrderDataMapper(input);

      EntityUser workerChackout = await GetUserFromDB(input.WorkerEmail, ErrorTypes.BadRequest, "Worker not found");

      newOrder.WorkerId = workerChackout.Id;

      var products = await GetOrderProductList(input, newOrder);

      newOrder.Products = products;

      Order order = new Order(newOrder);

      EntityOrder entityOrder = RequestMapper.MapToModel(order);

      await _repository.AddAsync<EntityOrder>(entityOrder);

      return new OperationResult(true);
    }

    public async Task<IEnumerable<EntityOrder>> GetEmployeeOrdersForDayAsync()
    {
      HttpContext httpContext = GetHttpContext();

      string email = GetEmailFromUserClaims(httpContext);

      EntityUser user = await GetUserFromDB(email, ErrorTypes.ServerError, "Server error");

      IQueryable<EntityOrder> orders = GetOrdersByDescendingWorkerPerDay(user);

      int numberOrders = GetNumberOrders(user);

      var result = await SetOrderNumber(orders, numberOrders);

      return result;
    }

    public IQueryable<EntityOrder> GetOrders() =>
      _repository.GetQueryable<EntityOrder>();
  }
}