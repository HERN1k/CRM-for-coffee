using CRM.Core.Entities.BaseEntities;
using CRM.Core.Interfaces.Entity;

namespace CRM.Core.Entities
{
  public class EntityProductCategory : BaseEntity, IEntityWithId, IEntityWithName
  {
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public List<EntityProduct>? Products { get; set; }
  }
}