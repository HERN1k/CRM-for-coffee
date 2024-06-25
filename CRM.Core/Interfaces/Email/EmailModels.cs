namespace CRM.Core.Interfaces.Email
{
  public class ConfirmRegister
  {
    public string TitleCode { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string TitleLink { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
  }

  public class RecoveryPassword
  {
    public string Password { get; set; } = string.Empty;
  }

  public class AddedNewManagerOrAdmin
  {
    public string Post { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string WorkerId { get; set; } = string.Empty;
  }
}