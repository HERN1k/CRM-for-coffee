namespace CRM.Application.Types
{
  public interface IEmailService
  {
    Task SendEmailConfirmRegister(string name, string toEmail, string code, string url);
    Task SendEmailUpdatePassword(string name, string toEmail);
    Task SendEmailRecoveryPassword(string name, string toEmail, string password);
  }
}