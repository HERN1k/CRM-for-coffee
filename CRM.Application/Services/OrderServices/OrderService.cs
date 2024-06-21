using System.Security.Claims;

using CRM.Application.RegEx;
using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.OrderServices;
using CRM.Core.Interfaces.Repositories;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services.OrderServices
{
  public class OrderService : IOrderService
  {
    private Order? _order { get; set; }
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly BusinessInformationSettings _businessInfSettings;
    private readonly IRepository _repository;

    public OrderService(
        IHttpContextAccessor httpContextAccessor,
        IOptions<BusinessInformationSettings> businessInfSettings,
        IRepository repository
      )
    {
      _httpContextAccessor = httpContextAccessor;
      _businessInfSettings = businessInfSettings.Value;
      _repository = repository;
    }

    #region CreateOrder
    public void ValidateOrderData(CreateOrderRequest request)
    {
      bool paymentMethod = Enum.IsDefined(typeof(PaymentMethods), request.PaymentMethod);
      if (!paymentMethod)
        throw new CustomException(ErrorTypes.ValidationError, "Payment method is incorrect or null");

      bool workerEmail = RegExHelper.ChackString(request.WorkerEmail, RegExPatterns.Email);
      if (!workerEmail)
        throw new CustomException(ErrorTypes.ValidationError, "Email is incorrect or null");

      foreach (var product in request.Products)
      {
        if (product.Amount <= 0)
          throw new CustomException(ErrorTypes.ValidationError, "Product amount is incorrect");

        if (product.AddOns.Count > 0)
        {
          foreach (var addOn in product.AddOns)
          {
            if (addOn.Amount <= 0)
              throw new CustomException(ErrorTypes.ValidationError, "Addon amount is incorrect");
          }
        }
      }
    }
    #endregion

    #region CreateOrder
    public async Task CreateOrder(CreateOrderRequest request)
    {
      var newOrder = new CreateOrder
      {
        TaxPercentage = _businessInfSettings.Taxes,
        PaymentMethod = (PaymentMethods)request.PaymentMethod
      };

      var workerChackout = await _repository.FindSingleAsync<EntityUser>(e => e.Email == request.WorkerEmail);
      if (workerChackout == null)
        throw new CustomException(ErrorTypes.BadRequest, "Worker not found");

      newOrder.WorkerId = workerChackout.Id;

      var products = new List<OrderProduct>();
      foreach (var product in request.Products)
      {
        var entityProduct = await _repository.FindSingleAsync<EntityProduct>(e => e.Id == product.ProductId);
        if (entityProduct == null)
          throw new CustomException(ErrorTypes.BadRequest, "Product not found");

        var tempProduct = new OrderProduct
        {
          Amount = product.Amount,
          ProductId = product.ProductId,
          AddOns = new List<OrderAddOn>()
        };

        newOrder.Total += tempProduct.Amount * entityProduct.Price;

        if (product.AddOns.Count > 0)
        {
          foreach (var addOn in product.AddOns)
          {
            var entityAddOn = await _repository.FindSingleAsync<EntityAddOn>(e => e.Id == addOn.AddOnId);
            if (entityAddOn == null)
              throw new CustomException(ErrorTypes.BadRequest, "Addon not found");

            var tempAddOn = new OrderAddOn
            {
              Amount = addOn.Amount,
              AddOnId = addOn.AddOnId
            };

            newOrder.Total += tempAddOn.Amount * entityAddOn.Price;
            tempProduct.AddOns.Add(tempAddOn);
          }
        }

        products.Add(tempProduct);
      }
      newOrder.Products = products;

      _order = new Order(newOrder);
    }
    #endregion

    #region SaveOrder
    public async Task SaveOrder()
    {
      if (_order == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      var newOrder = new EntityOrder
      {
        Total = _order.Total,
        Taxes = _order.Taxes,
        PaymentMethod = (int)_order.PaymentMethod,
        WorkerId = _order.WorkerId,
        OrderСreationDate = _order.OrderСreationDate,
        Products = new List<EntityOrderProduct>()
      };

      foreach (var product in _order.Products)
      {
        var tempProduct = new EntityOrderProduct
        {
          ProductId = product.ProductId,
          Amount = product.Amount,
          OrderId = newOrder.Id,
          AddOns = new List<EntityOrderAddOn>()
        };

        foreach (var addOn in product.AddOns)
        {
          var tempAddOn = new EntityOrderAddOn
          {
            AddOnId = addOn.AddOnId,
            Amount = addOn.Amount,
            OrderProductId = tempProduct.Id
          };

          tempProduct.AddOns.Add(tempAddOn);
        }

        newOrder.Products.Add(tempProduct);
      }

      await _repository.AddAsync<EntityOrder>(newOrder);
    }
    #endregion

    #region GetEmployeeOrdersForDay
    public async Task<IEnumerable<EntityOrder>> GetEmployeeOrdersForDay()
    {
      var httpContext = _httpContextAccessor.HttpContext
        ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      string email = httpContext.User.Claims
       .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;

      if (string.IsNullOrEmpty(email))
        throw new CustomException(ErrorTypes.BadRequest, "Email not found");

      var user = await _repository.FindSingleAsync<EntityUser>(e => e.Email == email)
        ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      var orders = _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Where(e => e.OrderСreationDate.Date == DateTime.UtcNow.Date)
        .Where(e => e.WorkerId == user.Id)
        .Include(e => e.Products.AsQueryable())
          .ThenInclude(e => e.AddOns)
        .OrderByDescending(e => e.OrderСreationDate);

      int countOrders = _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Where(e => e.OrderСreationDate.Date == DateTime.UtcNow.Date)
        .Where(e => e.WorkerId == user.Id)
        .Count();

      var result = await orders.ToListAsync();
      for (int i = 0; i < countOrders; i++)
      {
        result[i].OrderNumber = countOrders - i;
      }

      return result;
    }
    #endregion

    #region GetOrders
    public IQueryable<EntityOrder> GetOrders() =>
      _repository.GetQueryable<EntityOrder>();
    #endregion  
  }
}