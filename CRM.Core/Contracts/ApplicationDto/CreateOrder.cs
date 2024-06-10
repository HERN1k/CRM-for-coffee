using CRM.Core.Enums;
using CRM.Core.Models;

namespace CRM.Core.Contracts.ApplicationDto
{
  public class CreateOrder
  {
    public List<OrderProduct> Products { get; set; } = null!;
    public decimal Total { get; set; }
    public float TaxPercentage { get; set; }
    public PaymentMethods PaymentMethod { get; set; }
    public Guid WorkerId { get; set; }
  }
}