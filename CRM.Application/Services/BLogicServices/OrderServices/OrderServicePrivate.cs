using System.Security.Claims;

using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Contracts.GraphQlDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Entity;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Services.OrderServices
{
  public partial class OrderService
  {
    private async Task<EntityUser> GetUserFromDB(string email, ErrorTypes type, string message)
    {
      var result = await _repository
        .FindSingleAsync<EntityUser>(e => e.Email == email)
          ?? throw new CustomException(type, message);
      return result;
    }
    private async Task<T> GetItemFromDBById<T>(Guid id, ErrorTypes type, string message) where T : class, IEntityWithId
    {
      var result = await _repository
        .FindSingleAsync<T>(e => e.Id == id)
          ?? throw new CustomException(type, message);
      return result;
    }
    private CreateOrder CreateOrderDataMapper(CreateOrderRequest request)
    {
      return new CreateOrder
      {
        TaxPercentage = _businessInfSettings.Taxes,
        PaymentMethod = (PaymentMethods)request.PaymentMethod
      };
    }
    private async Task<List<OrderProduct>> GetOrderProductList(CreateOrderRequest request, CreateOrder newOrder)
    {
      var products = new List<OrderProduct>();

      foreach (var product in request.Products)
      {
        OrderProduct tempProduct = await GetOrderProduct(product, newOrder);

        products.Add(tempProduct);
      }

      return products;
    }
    private async Task<OrderProduct> GetOrderProduct(OrderProduct product, CreateOrder newOrder)
    {
      var entityProduct = await GetItemFromDBById<EntityProduct>(product.ProductId, ErrorTypes.BadRequest, "Product not found");

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
          await GetOrderAddOn(addOn, tempProduct, newOrder);
        }
      }

      return tempProduct;
    }
    private async Task GetOrderAddOn(OrderAddOn addOn, OrderProduct tempProduct, CreateOrder newOrder)
    {
      var entityAddOn = await GetItemFromDBById<EntityAddOn>(addOn.AddOnId, ErrorTypes.BadRequest, "Addon not found");
      var tempAddOn = new OrderAddOn
      {
        Amount = addOn.Amount,
        AddOnId = addOn.AddOnId
      };
      newOrder.Total += tempAddOn.Amount * entityAddOn.Price;
      tempProduct.AddOns.Add(tempAddOn);
    }
    private HttpContext GetHttpContext()
    {
      var httpContext = _httpContextAccessor.HttpContext
        ?? throw new CustomException(ErrorTypes.ServerError, "Server error");
      return httpContext;
    }
    private string GetEmailFromUserClaims(HttpContext httpContext)
    {
      string? email = httpContext.User.Claims
       .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;

      if (string.IsNullOrEmpty(email))
        throw new CustomException(ErrorTypes.BadRequest, "Email not found");

      return email;
    }
    private IQueryable<EntityOrder> GetOrdersByDescendingWorkerPerDay(EntityUser user)
    {
      return _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Where(e => e.OrderСreationDate.Date == DateTime.UtcNow.Date)
        .Where(e => e.WorkerId == user.Id)
        .Include(e => e.Products.AsQueryable())
          .ThenInclude(e => e.AddOns)
        .OrderByDescending(e => e.OrderСreationDate);
    }
    private int GetNumberOrders(EntityUser user)
    {
      return _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Where(e => e.OrderСreationDate.Date == DateTime.UtcNow.Date)
        .Where(e => e.WorkerId == user.Id)
        .Count();
    }
    private async Task<List<EntityOrder>> SetOrderNumber(IQueryable<EntityOrder> orders, int numberOrders)
    {
      var result = await orders.ToListAsync();
      for (int i = 0; i < numberOrders; i++)
      {
        result[i].OrderNumber = numberOrders - i;
      }
      return result;
    }
  }
}