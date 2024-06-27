using CRM.Core.Entities.BaseEntities;
using CRM.Core.Interfaces.Entity;

namespace CRM.Core.Entities
{
  public class EntityOrderProduct : BaseEntity, IEntityWithId
  {
    public Guid ProductId { get; set; }
    public int Amount { get; set; }
    public Guid OrderId { get; set; }
    public EntityOrder? Order { get; set; }
    public List<EntityOrderAddOn> AddOns { get; set; } = new List<EntityOrderAddOn>();
  }
}