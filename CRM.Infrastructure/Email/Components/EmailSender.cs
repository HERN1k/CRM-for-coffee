using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Infrastructure.Email;
using CRM.Core.Interfaces.Settings;
using CRM.Infrastructure.Email.Services;

using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;

using RazorLight;

namespace CRM.Infrastructure.Email.Components
{
    public class EmailSender(
      ILogger<EmailService> logger,
      IOptions<EmailSettings> emailSettings,
      IRazorLightEngine razorEngine
    ) : IEmailSender
  {
    private readonly ILogger<EmailService> _logger = logger;
    private readonly EmailSettings _emailSettings = emailSettings.Value;
    private readonly IRazorLightEngine _razorEngine = razorEngine;

    public async Task<string> CompileHtmlStringAsync<T>(string key, T model) where T : class
    {
      return await _razorEngine.CompileRenderAsync(key, model);
    }

    public async Task SendEmailAsync(string name, string email, string html)
    {
      var message = CreateMessage(name, email, html);
      await SendAsync(message);
    }

    /// <summary>
    ///   Creates an email message with the specified recipient's name, email address, and HTML content.
    /// </summary>
    /// <param name="name">The name of the recipient.</param>
    /// <param name="email">The email address of the recipient.</param>
    /// <param name="html">The HTML content of the email body.</param>
    /// <returns>A <see cref="MimeMessage"/> object representing the email message.</returns>
    private MimeMessage CreateMessage(string name, string email, string html)
    {
      var body = new TextPart(MimeKit.Text.TextFormat.Html)
      {
        Text = html
      };

      var message = new MimeMessage();
      message.From.Add(new MailboxAddress(_emailSettings.Title, _emailSettings.Address));
      message.To.Add(new MailboxAddress(name, email));
      message.Subject = "Bakery";
      message.Body = body;
      return message;
    }

    /// <summary>
    ///   Sends an email message asynchronously.
    /// </summary>
    /// <param name="message">The <see cref="MimeMessage"/> object representing the email message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="CustomException">Thrown when an exception occurs while sending the email.</exception>
    private async Task SendAsync(MimeMessage message)
    {
      using var client = new SmtpClient();
      try
      {
        client.Connect(_emailSettings.Server, _emailSettings.Port, SecureSocketOptions.StartTls);
        client.Authenticate(_emailSettings.Address, _emailSettings.Password);
        await client.SendAsync(message);
      }
      catch (MessageNotFoundException ex)
      {
        _logger.LogError(ex, "MailKit message not found exception");
        throw new CustomException(ErrorTypes.MailKitException, ex.Message);
      }
      catch (ProtocolException ex)
      {
        _logger.LogError(ex, "MailKit protocol exception");
        throw new CustomException(ErrorTypes.MailKitException, ex.Message);
      }
      catch (ServiceNotAuthenticatedException ex)
      {
        _logger.LogError(ex, "MailKit service not authenticated exception");
        throw new CustomException(ErrorTypes.MailKitException, ex.Message);
      }
      catch (ServiceNotConnectedException ex)
      {
        _logger.LogError(ex, "MailKit service not connected exception");
        throw new CustomException(ErrorTypes.MailKitException, ex.Message);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unexpected MailKit exception");
        throw new CustomException(ErrorTypes.MailKitException, ex.Message);
      }
      finally
      {
        client.Disconnect(true);
        client.Dispose();
      }
    }
  }
}