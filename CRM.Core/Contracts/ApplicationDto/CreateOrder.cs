using CRM.Core.Enums;
using CRM.Core.Models;

namespace CRM.Core.Contracts.ApplicationDto
{
  public class CreateOrder
  {
    public List<Product> Products { get; set; } = null!;
    public float TaxPercentage { get; set; }
    public PaymentMethods PaymentMethod { get; set; }
    public string WorkerChackoutId { get; set; } = string.Empty;
    public string WorkerKitchenId { get; set; } = string.Empty;
  }
}