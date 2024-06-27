using System.Text;

using CRM.Core.Entities;
using CRM.Core.Interfaces.Email;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

namespace CRM.Application.EmailManager
{
  /// <summary>
  ///   Provides methods for sending emails
  /// </summary>
  public partial class EmailSender(IEmailService emailService) : IEmailSender
  {
    private readonly IEmailService _emailService = emailService;

    /// <summary>
    ///   Sends an email to confirm the registration of a new user
    /// </summary>
    public async Task SendEmailConfirmRegisterAsync(User user, HttpSettings httpSettings, EmailConfirmRegisterSettings emailConfirmRegisterSettings)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(user.Email);
      string code = Convert.ToBase64String(bytes);
      string url = new StringBuilder()
        .Append(httpSettings.Protocol)
        .Append("://")
        .Append(httpSettings.Domaine)
        .Append("/Api/Auth/ConfirmRegister/")
        .Append(code)
        .ToString();

      string html = await _emailService
        .CompileHtmlStringAsync("ConfirmRegister", new ConfirmRegister
        {
          TitleCode = emailConfirmRegisterSettings.TitleCode,
          Code = code,
          TitleLink = emailConfirmRegisterSettings.TitleLink,
          Link = url
        });

      await _emailService.SendEmailAsync(user.FirstName, user.Email, html);
    }

    /// <summary>
    ///   Sends notification emails to administrators when a new manager is registered in the CRM
    /// </summary>
    public async Task NotifyAdminsOnManagerRegistration(EntityUser user, List<EntityUser> admins)
    {
      if (user.Post == "Worker")
        return;

      string html = await _emailService
        .CompileHtmlStringAsync("AddedNewManagerOrAdmin", new AddedNewManagerOrAdmin
        {
          Post = user.Post.ToLower(),
          Name = $"{user.FirstName} {user.LastName} {user.FatherName}",
          WorkerId = user.Id.ToString()
        });

      foreach (var admin in admins)
      {
        await _emailService.SendEmailAsync(admin.FirstName, admin.Email, html);
      }
    }

    /// <summary>
    ///   Sends an email to update password
    /// </summary>
    public async Task SendUpdatePasswordEmail(User user)
    {
      string html = await _emailService
        .CompileHtmlStringAsync("UpdatePassword", new { });

      await _emailService.SendEmailAsync(user.FirstName, user.Email, html);
    }

    /// <summary>
    ///   Sends an email to recovery password
    /// </summary>
    public async Task SendRecoveryPasswordEmail(User user, string password)
    {
      string html = await _emailService.CompileHtmlStringAsync("RecoveryPassword", new RecoveryPassword
      {
        Password = password
      });

      await _emailService.SendEmailAsync(user.FirstName, user.Email, html);
    }
  }
}