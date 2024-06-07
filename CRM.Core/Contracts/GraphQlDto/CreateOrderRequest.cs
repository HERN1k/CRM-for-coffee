using CRM.Core.Models;

namespace CRM.Core.Contracts.GraphQlDto
{
  public class CreateOrderRequest
  {
    public List<Product> Products { get; set; } = null!;
    public string PaymentMethod { get; set; } = string.Empty;
  }
}