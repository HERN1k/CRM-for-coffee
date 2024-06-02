using CRM.Core.Entities.BaseEntities;

namespace CRM.Core.Entities
{
  public class EntityRefreshToken : BaseEntity
  {
    //public Guid UserId { get; set; }
    public EntityUser User { get; set; } = null!;
    public string RefreshTokenString { get; set; } = null!;
  }
}
