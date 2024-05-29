namespace CRM.Core.Entities
{
  public class AddOn
  {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Amount { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
  }
}