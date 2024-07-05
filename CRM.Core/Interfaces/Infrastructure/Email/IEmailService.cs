using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

namespace CRM.Core.Interfaces.Infrastructure.Email
{
  /// <summary>
  ///   Provides methods for sending various types of emails using an email sender.
  /// </summary>
  public interface IEmailService
  {
    /// <summary>
    ///   Sends an email to confirm user registration.
    /// </summary>
    /// <param name="user">The user who is registering.</param>
    /// <param name="httpSettings">The HTTP settings for building the confirmation URL.</param>
    /// <param name="emailConfirmRegisterSettings">The settings for the email confirmation registration.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendEmailConfirmRegisterAsync(User user, HttpSettings httpSettings, EmailConfirmRegisterSettings emailConfirmRegisterSettings);

    /// <summary>
    ///   Notifies administrators about the registration of a new manager.
    /// </summary>
    /// <param name="user">The user who has registered.</param>
    /// <param name="admins">The list of administrators to notify.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NotifyAdminsOnManagerRegistration(User user, List<User> admins);

    /// <summary>
    ///   Sends an email to a user to notify them of a password update.
    /// </summary>
    /// <param name="user">The user who updated their password.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendUpdatePasswordEmail(User user);

    /// <summary>
    ///   Sends an email to a user with a recovered password.
    /// </summary>
    /// <param name="user">The user who is recovering their password.</param>
    /// <param name="password">The new password.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendRecoveryPasswordEmail(User user, string password);
  }
}