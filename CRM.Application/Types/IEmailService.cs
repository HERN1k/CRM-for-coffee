namespace CRM.Application.Types
{
  public interface IEmailService
  {
    Task<bool> SendEmailConfirmRegister(string name, string toEmail, string code, string url);
    Task<bool> SendEmailUpdatePassword(string name, string toEmail);
    Task<bool> SendEmailRecoveryPassword(string name, string toEmail, string password);
  }
}