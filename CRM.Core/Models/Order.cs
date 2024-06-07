using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Enums;
using CRM.Core.Models.BaseModels;

namespace CRM.Core.Models
{
  public class Order : BaseModel
  {
    public List<Product> Products { get; set; } = null!;
    public decimal Taxes { get; set; }
    public decimal Total { get; set; }
    public PaymentMethods PaymentMethod { get; set; }
    public string WorkerChackoutId { get; set; } = string.Empty;
    public string WorkerKitchenId { get; set; } = string.Empty;
    public bool IsSuccess { get; set; } = false;
    public DateTime OrderReceiptDate { get; set; } = DateTime.Now;
    public DateTime OrderIssueDate { get; set; }

    public Order(CreateOrder order)
    {
      Products = order.Products;
      PaymentMethod = order.PaymentMethod;
      WorkerChackoutId = order.WorkerChackoutId;
      WorkerKitchenId = order.WorkerKitchenId;
      Total = 0;
      foreach (var product in Products)
      {
        Total += product.Price;
        if (product.AddOns != null)
        {
          foreach (var addOn in product.AddOns)
          {
            Total += addOn.Price;
          }
        }
      }
      Taxes = Total * (decimal)order.TaxPercentage;
    }
  }
}