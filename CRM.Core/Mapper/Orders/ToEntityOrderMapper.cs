using AutoMapper;

using CRM.Core.Entities;
using CRM.Core.Models;

namespace CRM.Core.Mapper.Orders
{
  public class ToEntityOrderMapper : ITypeConverter<Order, EntityOrder>
  {
    public EntityOrder Convert(Order source, EntityOrder destination, ResolutionContext context)
    {
      EntityOrder result = new EntityOrder
      {
        Total = source.Total,
        Taxes = source.Taxes,
        PaymentMethod = (int)source.PaymentMethod,
        WorkerId = source.WorkerId,
        OrderСreationDate = source.OrderСreationDate,
        Products = new List<EntityOrderProduct>()
      };

      foreach (var product in source.Products)
      {
        var tempProduct = new EntityOrderProduct
        {
          ProductId = product.ProductId,
          Amount = product.Amount,
          OrderId = result.Id,
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

        result.Products.Add(tempProduct);
      }

      return result;
    }
  }
}