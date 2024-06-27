namespace CRM.Core.Models
{
  public class JwtTokenClaims
  {
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Post { get; set; } = null!;
  }
}