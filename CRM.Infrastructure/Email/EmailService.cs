using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.Email;
using CRM.Core.Interfaces.Settings;
using CRM.Infrastructure.Email.EmailModels;

using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;

using RazorLight;

namespace CRM.Infrastructure.Email
{
  public class EmailService : IEmailService
  {
    private readonly ILogger<EmailService> _logger;
    private readonly EmailSettings _emailSettings;
    private readonly EmailConfirmRegisterSettings _emailConfirmRegisterSettings;
    private readonly IRazorLightEngine _razorEngine;

    public EmailService(
        ILogger<EmailService> logger,
        IOptions<EmailSettings> emailSettings,
        IOptions<EmailConfirmRegisterSettings> emailConfirmRegisterSettings,
        IRazorLightEngine razorEngine
      )
    {
      _logger = logger;
      _emailSettings = emailSettings.Value;
      _emailConfirmRegisterSettings = emailConfirmRegisterSettings.Value;
      _razorEngine = razorEngine;
    }

    private MimeMessage CreateMessage(string name, string toEmail, TextPart body)
    {
      var message = new MimeMessage();
      message.From.Add(new MailboxAddress(_emailSettings.Title, _emailSettings.Address));
      message.To.Add(new MailboxAddress(string.Empty, toEmail));
      message.Subject = $"Hello {name}";
      message.Body = body;
      return message;
    }

    private void SendEmail(MimeMessage message)
    {
      using var client = new SmtpClient();
      try
      {
        client.Connect(_emailSettings.Server, _emailSettings.Port, SecureSocketOptions.StartTls);
        client.Authenticate(_emailSettings.Address, _emailSettings.Password);
        client.Send(message);
      }
      catch (MessageNotFoundException ex)
      {
        _logger.LogError(ex, "MailKit message not found exception");
        throw new CustomException(ErrorTypes.MailKitException, "MailKit exception", ex);
      }
      catch (ProtocolException ex)
      {
        _logger.LogError(ex, "MailKit protocol exception");
        throw new CustomException(ErrorTypes.MailKitException, "MailKit exception", ex);
      }
      catch (ServiceNotAuthenticatedException ex)
      {
        _logger.LogError(ex, "MailKit service not authenticated exception");
        throw new CustomException(ErrorTypes.MailKitException, "MailKit exception", ex);
      }
      catch (ServiceNotConnectedException ex)
      {
        _logger.LogError(ex, "MailKit service not connected exception");
        throw new CustomException(ErrorTypes.MailKitException, "MailKit exception", ex);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unexpected MailKit exception");
        throw new CustomException(ErrorTypes.MailKitException, "MailKit exception", ex);
      }
      finally
      {
        client.Disconnect(true);
        client.Dispose();
      }
    }

    public async Task SendEmailConfirmRegister(string name, string toEmail, string code, string url)
    {
      var model = new ConfirmRegister
      {
        TitleCode = _emailConfirmRegisterSettings.TitleCode,
        Code = code,
        TitleLink = _emailConfirmRegisterSettings.TitleLink,
        Link = url
      };
      string htmlString = await _razorEngine.CompileRenderAsync("ConfirmRegister", model);
      var body = new TextPart(MimeKit.Text.TextFormat.Html)
      {
        Text = htmlString
      };
      var message = CreateMessage(name, toEmail, body);
      SendEmail(message);
    }

    public async Task SendEmailUpdatePassword(string name, string toEmail)
    {
      string htmlString = await _razorEngine.CompileRenderAsync("UpdatePassword", new { });
      var body = new TextPart(MimeKit.Text.TextFormat.Html)
      {
        Text = htmlString
      };
      var message = CreateMessage(name, toEmail, body);
      SendEmail(message);
    }

    public async Task SendEmailRecoveryPassword(string name, string toEmail, string password)
    {
      var model = new RecoveryPassword
      {
        Password = password
      };
      string htmlString = await _razorEngine.CompileRenderAsync("RecoveryPassword", model);
      var body = new TextPart(MimeKit.Text.TextFormat.Html)
      {
        Text = htmlString
      };
      var message = CreateMessage(name, toEmail, body);
      SendEmail(message);
    }
  }
}