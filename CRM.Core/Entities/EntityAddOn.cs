using CRM.Core.Entities.BaseEntities;

namespace CRM.Core.Entities
{
  public class EntityAddOn : BaseProductEntity
  {
    public Guid ProductId { get; set; }
    public EntityProduct? Product { get; set; }
  }
}