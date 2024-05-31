using System.Security.Claims;
using System.Text;

using CRM.Application.RegEx;
using CRM.Application.Types;
using CRM.Application.Types.Options;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Models;
using CRM.Data.Types;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services
{
  public class AuthSundryService : IAuthSundryService
  {
    private readonly JwtOptions _jwtOptions;
    private readonly ITokenService _tokenService;
    private readonly IHesherService _hesherService;
    private readonly IAuthSundryStore _authSundryStore;
    private readonly IEmailService _emailService;
    private User? _user { get; set; }

    public AuthSundryService(
        IOptions<JwtOptions> jwtOptions,
        ITokenService tokenService,
        IHesherService hesherService,
        IAuthSundryStore authSundryStore,
        IEmailService emailService
      )
    {
      _jwtOptions = jwtOptions.Value;
      _tokenService = tokenService;
      _hesherService = hesherService;
      _authSundryStore = authSundryStore;
      _emailService = emailService;
    }

    public void ValidationEmail(string email)
    {
      bool result = RegExHelper.ChackString(email, RegExPatterns.Email);
      if (!result)
        throw new Exception("Exception");
    }

    public async Task CheckImmutableToken(string email, string refreshToken)
    {
      var user = await _authSundryStore.FindUserByEmail(email);

      _user = new User
      {
        Id = user.Id,
        FirstName = user.FirstName,
        Email = user.Email,
        Post = user.Post
      };

      string token = await _authSundryStore.FindTokenById(_user.Id);

      if (token != refreshToken)
        throw new Exception("Exception");
    }

    public async Task ValidateToken(string token)
    {
      await _tokenService.ValidateToken(token);
    }

    public async Task RemoveRefreshToken()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      await _authSundryStore.RemoveRefreshToken(_user.Id);
    }

    public string GetJwtAccessToken()
    {
      if (_user == null)
        throw new Exception("Exception");

      int tokenLifetime = _jwtOptions.AccessTokenLifetime;

      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.NameIdentifier, _user.Id.ToString()),
        new Claim(ClaimTypes.Email, _user.Email),
        new Claim(ClaimTypes.Role, _user.Post),
      };
      string result = _tokenService.CreateJwtToken(claims, tokenLifetime);
      if (string.IsNullOrEmpty(result))
        throw new Exception("Exception");
      return result;
    }

    public void ValidateDataUpdatePassword(UpdatePasswordRequest request)
    {
      bool email = RegExHelper.ChackString(request.email, RegExPatterns.Email);
      if (!email)
        throw new CustomException(ErrorTypes.ValidationError, "Email is incorrect or null");

      bool password = RegExHelper.ChackString(request.password, RegExPatterns.Password);
      if (!password)
        throw new CustomException(ErrorTypes.ValidationError, "Password is incorrect or null");

      bool newPassword = RegExHelper.ChackString(request.newPassword, RegExPatterns.Password);
      if (!newPassword)
        throw new CustomException(ErrorTypes.ValidationError, "New password is incorrect or null");
    }

    public async Task SetDataUpdatePassword(UpdatePasswordRequest request)
    {
      EntityUser user;
      try
      { user = await _authSundryStore.FindUserByEmail(request.email); }
      catch (Exception ex)
      {
        if (ex.Message == "Exception")
        {
          throw new CustomException(ErrorTypes.ServerError, "Server error");
        }
        throw;
      }

      _user = new User
      {
        Id = user.Id,
        Password = user.Password,
        RegistrationDate = user.RegistrationDate,
        Email = user.Email,
        FirstName = user.FirstName,
      };
    }

    public void VerificationPassword(string requestPassword)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      string processedSalt = _user.RegistrationDate.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string hash = _hesherService.GetHash(requestPassword, saltArray);
      bool result = hash == _user.Password;
      if (!result)
        throw new CustomException(ErrorTypes.BadRequest, "Incorrect password");
    }

    public CookieOptions SetCookieOptions()
    {
      return new CookieOptions
      {
        MaxAge = TimeSpan.FromMinutes(_jwtOptions.AccessTokenLifetime),
      };
    }

    public string GetHash(string requestPassword)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      string processedSalt = _user.RegistrationDate.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string result = _hesherService.GetHash(requestPassword, saltArray);
      if (string.IsNullOrEmpty(result))
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      return result;
    }

    public async Task SaveNewPassword(string password)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      await _authSundryStore.SaveNewPassword(_user.Email, password);
    }

    public async Task SendUpdatePasswordEmail()
    {
      try
      {
        if (_user == null)
          throw new CustomException(ErrorTypes.ServerError, "Server error");
        await _emailService.SendEmailUpdatePassword(_user.FirstName, _user.Email);
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
  }
}