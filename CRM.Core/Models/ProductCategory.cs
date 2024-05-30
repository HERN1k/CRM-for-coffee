namespace CRM.Core.Models
{
  public class ProductCategory
  {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
  }
}