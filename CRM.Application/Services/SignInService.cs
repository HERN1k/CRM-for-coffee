using System.Security.Claims;
using System.Text;

using CRM.Application.RegEx;
using CRM.Application.Types;
using CRM.Application.Types.Options;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
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

    public async Task SetData(string email)
    {
      var user = await _signInStore.FindUserByEmail(email);
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
        throw new CustomException(ErrorTypes.BadRequest, "Mail is unconfirmed");
    }

    public void ValidationDataSignIn(SignInRequest request)
    {
      bool email = RegExHelper.ChackString(request.email, RegExPatterns.Email);
      if (!email)
        throw new CustomException(ErrorTypes.ValidationError, "Email is incorrect or null");

      bool password = RegExHelper.ChackString(request.password, RegExPatterns.Password);
      if (!password)
        throw new CustomException(ErrorTypes.ValidationError, "Password is incorrect or null");
    }

    public void VerificationHash(string requestPassword)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      string processedSalt = _user.RegistrationDate.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string hash = _hashPassword.GetHash(requestPassword, saltArray);
      bool result = hash == _user.Password;
      if (!result)
        throw new CustomException(ErrorTypes.BadRequest, "Incorrect password");
    }

    public string GetJwtToken(TokenTypes typesTokens)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

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

    public async Task SaveToken(string token)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      await _signInStore.SaveToken(_user.Email, token);
    }

    public SignInResponse SetResponse(string refreshToken)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
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

    public CookieOptions SetCookieOptions(TokenTypes typesTokens)
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
