using CRM.Core.Interfaces.Entity;
using CRM.Core.Models.BaseModels;

namespace CRM.Core.Models
{
  public class AddOn : BaseModel, IEntityWithId, IEntityWithName
  {
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Amount { get; set; }
    public Guid ProductId { get; set; }
  }
}