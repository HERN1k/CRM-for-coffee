using CRM.Core.Entities.BaseEntities;
using CRM.Core.Interfaces.Entity;

namespace CRM.Core.Entities
{
  public class EntityRefreshToken : BaseEntity, IEntityWithId
  {
    public EntityUser User { get; set; } = null!;
    public string RefreshTokenString { get; set; } = null!;
  }
}
