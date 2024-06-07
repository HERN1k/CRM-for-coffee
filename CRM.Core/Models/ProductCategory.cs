using CRM.Core.Models.BaseModels;

namespace CRM.Core.Models
{
  public class ProductCategory : BaseModel
  {
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public List<Product> Products { get; set; } = null!;
  }
}