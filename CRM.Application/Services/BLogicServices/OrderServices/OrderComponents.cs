using System.Security.Claims;

using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Repositories.BLogicRepositories.OrderRepository;
using CRM.Core.Interfaces.Services.BLogicServices.OrderServices;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services.BLogicServices.OrderServices
{
  public class OrderComponents(
      IOrderRepository repository,
      IHttpContextAccessor httpContextAccessor,
      IOptions<BusinessInformationSettings> businessInfSettings
    ) : IOrderComponents
  {
    private readonly IOrderRepository _repository = repository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly BusinessInformationSettings _businessInfSettings = businessInfSettings.Value;

    public CreateOrderDto CreateOrderDataMapper(CreateOrderRequest request)
    {
      return new CreateOrderDto
      {
        TaxPercentage = _businessInfSettings.Taxes,
        PaymentMethod = (PaymentMethods)request.PaymentMethod
      };
    }

    public async Task<List<OrderProduct>> GetOrderProductList(CreateOrderRequest request, CreateOrderDto newOrder)
    {
      var products = new List<OrderProduct>();

      foreach (var product in request.Products)
      {
        OrderProduct tempProduct = await GetOrderProduct(product, newOrder);

        products.Add(tempProduct);
      }

      return products;
    }

    public async Task<OrderProduct> GetOrderProduct(OrderProduct product, CreateOrderDto newOrder)
    {
      var productItem = await _repository
        .FindEntityById<Product, EntityProduct>(product.ProductId, ErrorTypes.BadRequest, "Product not found");

      var tempProduct = new OrderProduct
      {
        Amount = product.Amount,
        ProductId = product.ProductId,
        AddOns = new List<OrderAddOn>()
      };

      newOrder.Total += tempProduct.Amount * productItem.Price;

      if (product.AddOns.Count > 0)
      {
        foreach (var addOn in product.AddOns)
        {
          await GetOrderAddOn(addOn, tempProduct, newOrder);
        }
      }

      return tempProduct;
    }

    public async Task GetOrderAddOn(OrderAddOn addOn, OrderProduct tempProduct, CreateOrderDto newOrder)
    {
      var addOnItem = await _repository
        .FindEntityById<AddOn, EntityAddOn>(addOn.AddOnId, ErrorTypes.BadRequest, "Addon not found");

      var tempAddOn = new OrderAddOn
      {
        Amount = addOn.Amount,
        AddOnId = addOn.AddOnId
      };

      newOrder.Total += tempAddOn.Amount * addOnItem.Price;

      tempProduct.AddOns.Add(tempAddOn);
    }

    public HttpContext GetHttpContext()
    {
      var httpContext = _httpContextAccessor.HttpContext
        ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      return httpContext;
    }

    public string GetEmailFromUserClaims(HttpContext httpContext)
    {
      string? email = httpContext.User.Claims
        .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
          ?? string.Empty;

      if (string.IsNullOrEmpty(email))
        throw new CustomException(ErrorTypes.BadRequest, "Email not found");

      return email;
    }

    public void SetOrderNumber(List<Order> orders, int numberOrders)
    {
      for (int i = 0; i < numberOrders; i++)
      {
        orders[i].OrderNumber = numberOrders - i;
      }
    }
  }
}