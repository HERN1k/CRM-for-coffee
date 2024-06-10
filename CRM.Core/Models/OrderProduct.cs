namespace CRM.Core.Models
{
  public class OrderProduct
  {
    public Guid ProductId { get; set; }
    public int Amount { get; set; }
    public List<OrderAddOn> AddOns { get; set; } = null!;
  }
}