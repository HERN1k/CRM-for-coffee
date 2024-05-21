namespace CRM.Infrastructure.Email.EmailModels
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
}
