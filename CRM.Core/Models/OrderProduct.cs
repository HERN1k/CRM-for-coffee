using CRM.Core.Interfaces.Entity;
using CRM.Core.Models.BaseModels;

namespace CRM.Core.Models
{
  public class OrderProduct : BaseModel, IEntityWithId
  {
    public Guid ProductId { get; set; }
    public int Amount { get; set; }
    public List<OrderAddOn> AddOns { get; set; } = null!;
  }
}