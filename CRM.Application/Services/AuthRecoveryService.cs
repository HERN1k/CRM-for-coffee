using System.Text;

using CRM.Application.RegEx;
using CRM.Application.Types;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Models;
using CRM.Data.Types;

namespace CRM.Application.Services
{
  public class AuthRecoveryService : IAuthRecoveryService
  {
    private readonly IAuthRecoveryStore _authRecoveryStore;
    private readonly IEmailService _emailService;
    private readonly IHesherService _hashPassword;
    private User? _user { get; set; }

    public AuthRecoveryService(
        IAuthRecoveryStore authRecoveryStore,
        IEmailService emailService,
        IHesherService hashPassword
      )
    {
      _authRecoveryStore = authRecoveryStore;
      _emailService = emailService;
      _hashPassword = hashPassword;
    }

    public void ValidationDataRecoveryPass(RecoveryPasswordRequest request)
    {
      bool email = RegExHelper.ChackString(request.email, RegExPatterns.Email);
      if (!email)
        throw new CustomException(ErrorTypes.ValidationError, "Email is incorrect or null");

      bool phoneNumber = RegExHelper.ChackString(request.phoneNumber, RegExPatterns.PhoneNumber);
      if (!phoneNumber)
        throw new CustomException(ErrorTypes.ValidationError, "Phone number is incorrect or null");

      bool post = RegExHelper.ChackString(request.post, RegExPatterns.Post);
      if (!post)
        throw new CustomException(ErrorTypes.ValidationError, "Post is incorrect or null");

      _user = new User
      {
        Email = request.email,
        PhoneNumber = request.phoneNumber,
        Post = request.post
      };
    }

    public async Task СomparisonRecoveryPassData(RecoveryPasswordRequest request)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      var user = await _authRecoveryStore.FindUserByEmail(_user.Email);
      _user.Id = user.Id;
      _user.Email = user.Email;
      _user.Post = user.Post;
      _user.PhoneNumber = user.PhoneNumber;
      _user.RegistrationDate = user.RegistrationDate;

      if (_user.PhoneNumber != request.phoneNumber)
        throw new CustomException(ErrorTypes.BadRequest, "Some data doesn't match");
      if (_user.Post != request.post)
        throw new CustomException(ErrorTypes.BadRequest, "Some data doesn't match");
    }

    public string GetNewPassword(int length)
    {
      const string validChars = "ABCDEFGHIJKLMNOabcdefghijkl12345PQRSTUVWXYZmnopqrstuvwxyz67890";
      const string validSigns = "#@$%&*-=";
      var result = new char[length];
      for (int i = 0; i < length; i++)
      {
        result[i] = validChars[_hashPassword.GetRandomNumber(0, validChars.Length - 1)];
      }
      for (int i = 0; i < 2; i++)
      {
        result[_hashPassword.GetRandomNumber(0, length - 1)] =
          validSigns[_hashPassword.GetRandomNumber(0, validSigns.Length - 1)];
      }
      string resultString = new string(result);
      if (string.IsNullOrEmpty(resultString))
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      return resultString;
    }
    public async Task SendRecoveryPassEmail(string password)
    {
      try
      {
        if (_user == null)
          throw new CustomException(ErrorTypes.ServerError, "Server error");
        await _emailService.SendEmailRecoveryPassword(_user.FirstName, _user.Email, password);
      }
      catch (CustomException ex)
      {
        if (ex.Message == "Email exception")
        {
          throw new CustomException(ErrorTypes.ServerError, "Server error");
        }
        throw;
      }
    }

    public async Task SaveNewPassword(string password)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      string processedSalt = _user.RegistrationDate.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string hash = _hashPassword.GetHash(password, saltArray);
      await _authRecoveryStore.SaveNewPassword(_user.Id, hash);
    }

    public async Task RemoveRefreshToken()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      await _authRecoveryStore.RemoveRefreshToken(_user.Id);
    }
  }
}