namespace CRM.Core.Entities
{
  public class EntityProduct
  {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Amount { get; set; }
    public Guid ProductCategoryId { get; set; }
    public EntityProductCategory? ProductCategory { get; set; }
    public List<EntityAddOn>? AddOns { get; set; }
  }
}