using CRM.Core.Models;

namespace CRM.Core.Contracts.GraphQlDto
{
  public class CreateOrderRequest
  {
    public List<OrderProduct> Products { get; set; } = null!;
    public int PaymentMethod { get; set; }
    public string WorkerChackoutEmail { get; set; } = string.Empty;
  }
}