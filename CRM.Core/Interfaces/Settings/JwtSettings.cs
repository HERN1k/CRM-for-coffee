namespace CRM.Core.Interfaces.Settings
{
  public class JwtSettings
  {
    public string IssuerJwt { get; set; } = "CRMServer";
    public string AudienceJwt { get; set; } = "CRMClient";
    public string SecurityKeyJwt { get; set; } = "mysupersecret_secretsecretsecretkey!123";
    public int ClockSkewJwt { get; set; } = 1;
    public int AccessTokenLifetime { get; set; } = 30;
    public int RefreshTokenLifetime { get; set; } = 1440;
  }
}
