using CRM.Core.Interfaces.Entity;
using CRM.Core.Models.BaseModels;

namespace CRM.Core.Models
{
  public class OrderAddOn : BaseModel, IEntityWithId
  {
    public Guid AddOnId { get; set; }
    public int Amount { get; set; }
  }
}