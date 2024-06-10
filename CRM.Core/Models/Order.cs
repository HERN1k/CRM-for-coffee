using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Enums;
using CRM.Core.Models.BaseModels;

namespace CRM.Core.Models
{
  public class Order : BaseModel
  {
    public List<OrderProduct> Products { get; set; } = null!;
    public decimal Taxes { get; set; }
    public decimal Total { get; set; } = 0;
    public PaymentMethods PaymentMethod { get; set; }
    public Guid WorkerId { get; set; }
    public DateTime OrderСreationDate { get; set; } = DateTime.UtcNow;

    public Order(CreateOrder order)
    {
      Products = order.Products;
      PaymentMethod = order.PaymentMethod;
      WorkerId = order.WorkerId;
      Total = order.Total;
      Taxes = Math.Round(Total * ((decimal)order.TaxPercentage / 100), 2);
    }
  }
}