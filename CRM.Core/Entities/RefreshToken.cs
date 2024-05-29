namespace CRM.Core.Entities
{
  public class RefreshToken
  {
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string RefreshTokenString { get; set; } = null!;
  }
}
