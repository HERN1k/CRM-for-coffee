using CRM.Application.Types;
using CRM.Application.Types.Options;
using CRM.Infrastructure.Email.EmailModels;

using LogLib.Types;

using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Options;

using MimeKit;

using RazorLight;

namespace CRM.Infrastructure.Email
{
  public class EmailService : IEmailService
  {
    private readonly ILoggerLib _logger;
    private readonly EmailOptions _emailOptions;
    private readonly EmailConfirmRegisterOptions _emailConfirmRegisterOptions;
    private readonly IRazorLightEngine _razorEngine;

    public EmailService(
        ILoggerLib logger,
        IOptions<EmailOptions> emailOptions,
        IOptions<EmailConfirmRegisterOptions> emailConfirmRegisterOptions,
        IRazorLightEngine razorEngine
      )
    {
      _logger = logger;
      _emailOptions = emailOptions.Value;
      _emailConfirmRegisterOptions = emailConfirmRegisterOptions.Value;
      _razorEngine = razorEngine;
    }

    private MimeMessage CreateMessage(string name, string toEmail, TextPart body)
    {
      var message = new MimeMessage();
      message.From.Add(new MailboxAddress(_emailOptions.Title, _emailOptions.Address));
      message.To.Add(new MailboxAddress(string.Empty, toEmail));
      message.Subject = $"Hello {name}";
      message.Body = body;
      return message;
    }

    private async Task<bool> SendEmail(MimeMessage message)
    {
      using var client = new SmtpClient();
      try
      {
        client.Connect(_emailOptions.Server, _emailOptions.Port, SecureSocketOptions.StartTls);
        client.Authenticate(_emailOptions.Address, _emailOptions.Password);
        client.Send(message);
      }
      catch (Exception ex)
      {
        await _logger.WriteErrorLog(ex);
        return false;
      }
      finally
      {
        client.Disconnect(true);
      }
      return true;
    }

    public async Task<bool> SendEmailConfirmRegister(string name, string toEmail, string code, string url)
    {
      var model = new ConfirmRegister
      {
        TitleCode = _emailConfirmRegisterOptions.TitleCode,
        Code = code,
        TitleLink = _emailConfirmRegisterOptions.TitleLink,
        Link = url
      };
      string htmlString = await _razorEngine.CompileRenderAsync("ConfirmRegister", model);
      var body = new TextPart(MimeKit.Text.TextFormat.Html)
      {
        Text = htmlString
      };
      var message = CreateMessage(name, toEmail, body);
      bool result = await SendEmail(message);
      return result;
    }

    public async Task<bool> SendEmailUpdatePassword(string name, string toEmail)
    {
      string htmlString = await _razorEngine.CompileRenderAsync("UpdatePassword", new { });
      var body = new TextPart(MimeKit.Text.TextFormat.Html)
      {
        Text = htmlString
      };
      var message = CreateMessage(name, toEmail, body);
      bool result = await SendEmail(message);
      return result;
    }

    public async Task<bool> SendEmailRecoveryPassword(string name, string toEmail, string password)
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
      bool result = await SendEmail(message);
      return result;
    }
  }
}