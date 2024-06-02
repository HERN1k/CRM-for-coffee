using CRM.Core.Entities.BaseEntities;

namespace CRM.Core.Entities
{
  public class EntityAddOn : BaseProductEntity
  {
    //public Guid Id { get; set; } = Guid.NewGuid();
    //public string Name { get; set; } = string.Empty;
    //public decimal Price { get; set; }
    //public int Amount { get; set; }
    public Guid ProductId { get; set; }
    public EntityProduct? Product { get; set; }
  }
}