using CRM.Core.Entities;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

namespace CRM.Core.Interfaces.Email
{
  public interface IEmailSender
  {
    Task SendEmailConfirmRegisterAsync(User user, HttpSettings httpSettings, EmailConfirmRegisterSettings emailConfirmRegisterSettings);
    Task NotifyAdminsOnManagerRegistration(EntityUser entityUser, List<EntityUser> admins);
    Task SendUpdatePasswordEmail(User user);
    Task SendRecoveryPasswordEmail(User user, string password);
  }
}