using CRM.Core.Entities.BaseEntities;

namespace CRM.Core.Entities
{
  public class EntityOrderAddOn : BaseEntity
  {
    public Guid AddOnId { get; set; }
    public int Amount { get; set; }
    public Guid OrderProductId { get; set; }
    public EntityOrderProduct? OrderProduct { get; set; }
  }
}