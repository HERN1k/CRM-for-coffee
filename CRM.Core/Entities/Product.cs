namespace CRM.Core.Entities
{
  public class Product
  {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Amount { get; set; }
    public Guid ProductCategoryId { get; set; }
    public ProductCategory? ProductCategory { get; set; }
    public List<AddOn>? AddOns { get; set; }
  }
}