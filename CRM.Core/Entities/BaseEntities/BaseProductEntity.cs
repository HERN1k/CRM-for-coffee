namespace CRM.Core.Entities.BaseEntities
{
  public abstract class BaseProductEntity : BaseEntity
  {
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Amount { get; set; }
  }
}