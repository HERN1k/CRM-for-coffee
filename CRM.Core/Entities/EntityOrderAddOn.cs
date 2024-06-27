using CRM.Core.Entities.BaseEntities;
using CRM.Core.Interfaces.Entity;

namespace CRM.Core.Entities
{
  public class EntityOrderAddOn : BaseEntity, IEntityWithId
  {
    public Guid AddOnId { get; set; }
    public int Amount { get; set; }
    public Guid OrderProductId { get; set; }
    public EntityOrderProduct? OrderProduct { get; set; }
  }
}