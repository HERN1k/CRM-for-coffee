using CRM.Core.Entities.BaseEntities;
using CRM.Core.Interfaces.Entity;

namespace CRM.Core.Entities
{
  public class EntityProduct : BaseProductEntity, IEntityWithId, IEntityWithName
  {
    public string Image { get; set; } = string.Empty;
    public Guid ProductCategoryId { get; set; }
    public EntityProductCategory? ProductCategory { get; set; }
    public List<EntityAddOn>? AddOns { get; set; }
  }
}