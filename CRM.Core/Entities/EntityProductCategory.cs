namespace CRM.Core.Entities
{
  public class EntityProductCategory
  {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public List<EntityProduct>? Products { get; set; }
  }
}