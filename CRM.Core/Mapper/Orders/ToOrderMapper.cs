using AutoMapper;

using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Models;

namespace CRM.Core.Mapper.Orders
{
  public class ToOrderMapper : ITypeConverter<EntityOrder, Order>
  {
    public Order Convert(EntityOrder source, Order destination, ResolutionContext context)
    {
      Order result = new Order
      {
        Id = source.Id,
        Taxes = source.Taxes,
        Total = source.Total,
        PaymentMethod = (PaymentMethods)source.PaymentMethod,
        WorkerId = source.WorkerId,
        OrderСreationDate = source.OrderСreationDate,
        OrderNumber = 0,
        Products = new List<OrderProduct>()
      };

      foreach (var product in source.Products)
      {
        var tempProduct = new OrderProduct
        {
          Id = product.Id,
          ProductId = product.ProductId,
          Amount = product.Amount,
          AddOns = new List<OrderAddOn>()
        };

        foreach (var addOn in product.AddOns)
        {
          var tempAddOn = new OrderAddOn
          {
            Id = addOn.Id,
            AddOnId = addOn.AddOnId,
            Amount = addOn.Amount
          };

          tempProduct.AddOns.Add(tempAddOn);
        }

        result.Products.Add(tempProduct);
      }

      return result;
    }
  }
}