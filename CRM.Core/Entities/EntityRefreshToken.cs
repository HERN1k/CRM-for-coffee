namespace CRM.Core.Entities
{
  public class EntityRefreshToken
  {
    public Guid UserId { get; set; }
    public EntityUser User { get; set; } = null!;
    public string RefreshTokenString { get; set; } = null!;
  }
}
