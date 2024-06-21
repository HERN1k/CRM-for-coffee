using CRM.Core.Entities.BaseEntities;

namespace CRM.Core.Entities
{
  public class EntityOrderProduct : BaseEntity
  {
    public Guid ProductId { get; set; }
    public int Amount { get; set; }
    public Guid OrderId { get; set; }
    public EntityOrder? Order { get; set; }
    public List<EntityOrderAddOn> AddOns { get; set; } = new List<EntityOrderAddOn>();
  }
}