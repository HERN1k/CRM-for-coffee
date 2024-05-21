using System.Security.Claims;
using System.Text;

using CRM.Application.RegEx;
using CRM.Application.Types;
using CRM.Application.Types.Options;
using CRM.Core.Contracts;
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
    private MainUser? _user { get; set; }

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

    public bool ValidationEmail(string email)
    {
      return RegExHelper.ChackString(email, RegExPatterns.Email);
    }

    public async Task<bool> CheckImmutableToken(string email, string refreshToken)
    {
      var user = await _authSundryStore.FindUserByEmail(email);
      if (user == null)
        return false;

      _user = new MainUser
      {
        Id = user.UserId,
        FirstName = user.FirstName,
        Email = user.Email,
        Post = user.Post
      };

      string token = await _authSundryStore.FindTokenById(_user.Id);
      if (string.IsNullOrEmpty(token))
        return false;

      if (token != refreshToken)
        return false;

      return true;
    }

    public async Task<bool> ValidateToken(string token)
    {
      return await _tokenService.ValidateToken(token);
    }

    public async Task<bool> RemoveRefreshToken()
    {
      if (_user == null)
        return false;
      var result = await _authSundryStore.RemoveRefreshToken(_user.Id);
      return result;
    }

    public string GetJwtAccessToken()
    {
      if (_user == null)
        return string.Empty;

      int tokenLifetime = _jwtOptions.AccessTokenLifetime;

      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, _user.Email),
        new Claim(ClaimTypes.Role, _user.Post),
      };

      return _tokenService.CreateJwtToken(claims, tokenLifetime);
    }

    public ValidationResult ValidateDataUpdatePassword(UpdatePasswordRequest request)
    {
      bool email = RegExHelper.ChackString(request.email, RegExPatterns.Email);
      if (!email)
        return new ValidationResult { IsSuccess = false, Field = "Email" };

      bool password = RegExHelper.ChackString(request.password, RegExPatterns.Password);
      if (!password)
        return new ValidationResult { IsSuccess = false, Field = "Password" };

      bool newPassword = RegExHelper.ChackString(request.newPassword, RegExPatterns.Password);
      if (!newPassword)
        return new ValidationResult { IsSuccess = false, Field = "New password" };

      return new ValidationResult { IsSuccess = true, Field = string.Empty };
    }

    public async Task<bool> SetDataUpdatePassword(UpdatePasswordRequest request)
    {
      var user = await _authSundryStore.FindUserByEmail(request.email);
      if (user == null)
        return false;
      _user = new MainUser
      {
        Id = user.UserId,
        Password = user.Password,
        RegistrationDate = user.RegistrationDate,
        Email = user.Email,
        FirstName = user.FirstName,
      };
      return true;
    }

    public bool VerificationPassword(string requestPassword)
    {
      if (_user == null)
        return false;
      string processedSalt = _user.RegistrationDate.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string hash = _hesherService.GetHash(requestPassword, saltArray);
      bool result = hash == _user.Password;
      return result;
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
        return string.Empty;
      string processedSalt = _user.RegistrationDate.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      return _hesherService.GetHash(requestPassword, saltArray);
    }

    public async Task<bool> SaveNewPassword(string password)
    {
      if (_user == null)
        return false;
      bool result = await _authSundryStore.SaveNewPassword(_user.Email, password);
      return result;
    }

    public async Task<bool> SendUpdatePasswordEmail()
    {
      if (_user == null)
        return false;
      bool result = await _emailService.SendEmailUpdatePassword(_user.FirstName, _user.Email);
      return result;
    }
  }
}