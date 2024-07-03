using System.Text;

using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Entities;
using CRM.Core.Interfaces.Infrastructure.Email;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

namespace CRM.Infrastructure.Email.Services
{
    public class EmailService(
        IEmailSender sender
      ) : IEmailService
  {
    private readonly IEmailSender _sender = sender;

    public async Task SendEmailConfirmRegisterAsync(User user, HttpSettings httpSettings, EmailConfirmRegisterSettings emailConfirmRegisterSettings)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(user.Email);
      string code = Convert.ToBase64String(bytes);
      string url = new StringBuilder()
        .Append(httpSettings.Protocol)
        .Append("://")
        .Append(httpSettings.Domaine)
        .Append("/api/auth/confirm_register/")
        .Append(code)
        .ToString();

      string html = await _sender
        .CompileHtmlStringAsync("ConfirmRegister", new ConfirmRegister
        {
          TitleCode = emailConfirmRegisterSettings.TitleCode,
          Code = code,
          TitleLink = emailConfirmRegisterSettings.TitleLink,
          Link = url
        });

      await _sender.SendEmailAsync(user.FirstName, user.Email, html);
    }

    public async Task NotifyAdminsOnManagerRegistration(EntityUser user, List<EntityUser> admins)
    {
      if (user.Post == "Worker")
        return;

      string html = await _sender
        .CompileHtmlStringAsync("AddedNewManagerOrAdmin", new AddedNewManagerOrAdmin
        {
          Post = user.Post.ToLower(),
          Name = $"{user.FirstName} {user.LastName} {user.FatherName}",
          WorkerId = user.Id.ToString()
        });

      foreach (var admin in admins)
      {
        await _sender.SendEmailAsync(admin.FirstName, admin.Email, html);
      }
    }

    public async Task SendUpdatePasswordEmail(User user)
    {
      string html = await _sender
        .CompileHtmlStringAsync("UpdatePassword", new { });

      await _sender.SendEmailAsync(user.FirstName, user.Email, html);
    }

    public async Task SendRecoveryPasswordEmail(User user, string password)
    {
      string html = await _sender.CompileHtmlStringAsync("RecoveryPassword", new RecoveryPassword
      {
        Password = password
      });

      await _sender.SendEmailAsync(user.FirstName, user.Email, html);
    }
  }
}