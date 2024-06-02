namespace CRM.Core.Entities.BaseEntities
{
  public abstract class BaseEntity
  {
    public Guid Id { get; set; } = Guid.NewGuid();
  }
}