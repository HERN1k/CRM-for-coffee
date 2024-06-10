using CRM.Core.Entities.BaseEntities;

namespace CRM.Core.Entities
{
  public class EntityProductCategory : BaseEntity
  {
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public List<EntityProduct>? Products { get; set; }
  }
}