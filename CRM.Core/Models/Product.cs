using CRM.Core.Models.BaseModels;

namespace CRM.Core.Models
{
  public class Product : BaseModel
  {
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Amount { get; set; }
    public Guid ProductCategoryId { get; set; }
    public List<AddOn>? AddOns { get; set; }
  }
}