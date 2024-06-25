namespace CRM.Core.Interfaces.Email
{
  public interface IEmailService
  {
    Task<string> CompileHtmlStringAsync<T>(string key, T model) where T : class;
    Task SendEmailAsync(string name, string email, string html);
  }
}