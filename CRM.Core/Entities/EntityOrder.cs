using CRM.Core.Entities.BaseEntities;
using CRM.Core.Interfaces.Entity;

namespace CRM.Core.Entities
{
  public class EntityOrder : BaseEntity, IEntityWithId
  {
    public List<EntityOrderProduct> Products { get; set; } = new List<EntityOrderProduct>();
    public decimal Taxes { get; set; }
    public decimal Total { get; set; }
    public int PaymentMethod { get; set; }
    public Guid WorkerId { get; set; }
    public DateTime OrderСreationDate { get; set; }
  }
}