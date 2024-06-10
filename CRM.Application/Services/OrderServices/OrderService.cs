using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.OrderServices;
using CRM.Core.Interfaces.Repositories;
using CRM.Core.Models;

namespace CRM.Application.Services.OrderServices
{
  public class OrderService : IOrderService
  {
    private Order? _order { get; set; }
    private readonly IRepository<EntityUser> _userRepository;
    private readonly IRepository<EntityProduct> _productRepository;
    private readonly IRepository<EntityAddOn> _addOnRepository;
    private readonly IRepository<EntityOrder> _orderRepository;

    public OrderService(
        IRepository<EntityUser> userRepository,
        IRepository<EntityProduct> productRepository,
        IRepository<EntityAddOn> addOnRepository,
        IRepository<EntityOrder> orderRepository
      )
    {
      _userRepository = userRepository;
      _productRepository = productRepository;
      _addOnRepository = addOnRepository;
      _orderRepository = orderRepository;
    }

    public void ValidateOrderData(CreateOrderRequest request)
    {
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

    public async Task CreateOrder(CreateOrderRequest request)
    {
      var newOrder = new CreateOrder
      {
        TaxPercentage = 19.5f, // change!
        PaymentMethod = (PaymentMethods)request.PaymentMethod
      };

      var workerChackout = await _userRepository.FindSingleAsync(e => e.Email == request.WorkerChackoutEmail);
      if (workerChackout == null)
        throw new CustomException(ErrorTypes.BadRequest, "Worker not found");

      newOrder.WorkerId = workerChackout.Id;

      var products = new List<OrderProduct>();
      foreach (var product in request.Products)
      {
        var entityProduct = await _productRepository.FindSingleAsync(e => e.Id == product.ProductId);
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
            var entityAddOn = await _addOnRepository.FindSingleAsync(e => e.Id == addOn.AddOnId);
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

      await _orderRepository.AddAsync(newOrder);
    }
  }
}