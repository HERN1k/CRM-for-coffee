using System.Security.Claims;
using System.Text;
using CRM.Application.RegEx;
using CRM.Application.Security;
using CRM.Application.Types;
using CRM.Application.Types.Options;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Models;
using CRM.Core.Responses;
using CRM.Data.Types;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services
{
    public class SignInService : ISignInService
  {
    private readonly JwtOptions _jwtOptions;
    private readonly ISignInStore _signInStore;
    private readonly IHesherService _hashPassword;
    private readonly ITokenService _tokenServices;
    private User? _user { get; set; }

    public SignInService(
        IOptions<JwtOptions> jwtOptions,
        ISignInStore signInStore,
        IHesherService hashPassword,
        ITokenService tokenServices
      )
    {
      _jwtOptions = jwtOptions.Value;
      _signInStore = signInStore;
      _hashPassword = hashPassword;
      _tokenServices = tokenServices;
    }

    public async Task<bool> SetData(string email)
    {
      var user = await _signInStore.FindUserByEmail(email);
      if (user == null)
        return false;
      _user = new User
      {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        FatherName = user.FatherName,
        Email = user.Email,
        Password = user.Password,
        Post = user.Post,
        Age = user.Age,
        Gender = user.Gender,
        PhoneNumber = user.PhoneNumber,
        IsConfirmed = user.IsConfirmed,
        RegistrationDate = user.RegistrationDate
      };
      if (!_user.IsConfirmed)
        return false;
      return true;
    }

    public ValidationResult ValidationDataSignIn(SignInRequest request)
    {
      bool email = RegExHelper.ChackString(request.email, RegExPatterns.Email);
      if (!email)
        return new ValidationResult { IsSuccess = false, Field = "Email" };

      bool password = RegExHelper.ChackString(request.password, RegExPatterns.Password);
      if (!password)
        return new ValidationResult { IsSuccess = false, Field = "Password" };

      return new ValidationResult { IsSuccess = true, Field = string.Empty };
    }

    public bool VerificationHash(string requestPassword)
    {
      if (_user == null)
        return false;
      string processedSalt = _user.RegistrationDate.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string hash = _hashPassword.GetHash(requestPassword, saltArray);
      bool result = hash == _user.Password;
      return result;
    }

    public string GetJwtToken(TypesToken typesTokens)
    {
      if (_user == null)
        return string.Empty;

      int tokenLifetime = 30;
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.NameIdentifier, _user.Id.ToString()),
        new Claim(ClaimTypes.Email, _user.Email),
        new Claim(ClaimTypes.Role, _user.Post),
      };

      if ((int)typesTokens == 1)
        tokenLifetime = _jwtOptions.AccessTokenLifetime;
      else if ((int)typesTokens == 2)
        tokenLifetime = _jwtOptions.RefreshTokenLifetime;

      return _tokenServices.CreateJwtToken(claims, tokenLifetime);
    }

    public async Task<bool> SaveToken(string token)
    {
      if (_user == null)
        return false;
      bool result = await _signInStore.SaveToken(_user.Email, token);
      return result;
    }

    public SignInResponse? SetResponse(string refreshToken)
    {
      if (_user == null)
        return null;
      return new SignInResponse
      {
        RefreshToken = refreshToken,
        FirstName = _user.FirstName,
        LastName = _user.LastName,
        FatherName = _user.FatherName,
        Email = _user.Email,
        Gender = _user.Gender,
        Post = _user.Post
      };
    }

    public CookieOptions SetCookieOptions(TypesToken typesTokens)
    {
      if ((int)typesTokens == 1)
      {
        return new CookieOptions
        {
          MaxAge = TimeSpan.FromMinutes(_jwtOptions.AccessTokenLifetime),
        };
      }
      else if ((int)typesTokens == 2)
      {
        return new CookieOptions
        {
          MaxAge = TimeSpan.FromMinutes(_jwtOptions.RefreshTokenLifetime),
        };
      }
      else
      {
        return new CookieOptions
        {
          MaxAge = TimeSpan.FromMinutes(_jwtOptions.AccessTokenLifetime),
        };
      }
    }
  }
}
