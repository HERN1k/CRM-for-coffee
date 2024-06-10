using CRM.Core.Entities.BaseEntities;

namespace CRM.Core.Entities
{
  public class EntityOrder : BaseEntity
  {
    public List<EntityOrderProduct>? Products { get; set; }
    public decimal Taxes { get; set; }
    public decimal Total { get; set; }
    public int PaymentMethod { get; set; }
    public Guid WorkerId { get; set; }
    public DateTime OrderСreationDate { get; set; }
  }
}