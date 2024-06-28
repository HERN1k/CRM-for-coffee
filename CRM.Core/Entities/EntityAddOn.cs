using CRM.Core.Entities.BaseEntities;
using CRM.Core.Interfaces.Entity;

namespace CRM.Core.Entities
{
  public class EntityAddOn : BaseProductEntity, IEntityWithId, IEntityWithName
  {
    public Guid ProductId { get; set; }
    public EntityProduct? Product { get; set; }
  }
}