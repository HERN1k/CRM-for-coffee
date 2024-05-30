using System.Text;

using CRM.Application.RegEx;
using CRM.Application.Types;
using CRM.Core.Contracts.RestDto;
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

    public ValidationResult ValidationDataRcoveryPass(RecoveryPasswordRequest request)
    {
      bool email = RegExHelper.ChackString(request.email, RegExPatterns.Email);
      if (!email)
        return new ValidationResult { IsSuccess = false, Field = "Email" };

      bool phoneNumber = RegExHelper.ChackString(request.phoneNumber, RegExPatterns.PhoneNumber);
      if (!phoneNumber)
        return new ValidationResult { IsSuccess = false, Field = "PhoneNumber" };

      bool post = RegExHelper.ChackString(request.post, RegExPatterns.Post);
      if (!post)
        return new ValidationResult { IsSuccess = false, Field = "Post" };

      _user = new User
      {
        Email = request.email,
        PhoneNumber = request.phoneNumber,
        Post = request.post
      };

      return new ValidationResult { IsSuccess = true, Field = string.Empty };
    }

    public async Task<bool> СomparisonRecoveryPassData()
    {
      if (_user == null)
        return false;
      var user = await _authRecoveryStore.FindUserByEmail(_user.Email);
      if (user == null)
        return false;
      if (_user.PhoneNumber != user.PhoneNumber)
        return false;
      if (_user.Post != user.Post)
        return false;
      _user.Id = user.Id;
      _user.Email = user.Email;
      _user.Post = user.Post;
      _user.RegistrationDate = user.RegistrationDate;
      return true;
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
      return new string(result);
    }
    public async Task<bool> SendRecoveryPassEmail(string password)
    {
      if (_user == null)
        return false;
      bool result = await _emailService.SendEmailRecoveryPassword(_user.FirstName, _user.Email, password);
      return result;
    }

    public async Task<bool> SaveNewPassword(string password)
    {
      if (_user == null)
        return false;
      string processedSalt = _user.RegistrationDate.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string hash = _hashPassword.GetHash(password, saltArray);
      bool result = await _authRecoveryStore.SaveNewPassword(_user.Id, hash);
      return result;
    }

    public async Task<bool> RemoveRefreshToken()
    {
      if (_user == null)
        return false;
      var result = await _authRecoveryStore.RemoveRefreshToken(_user.Id);
      return result;
    }
  }
}