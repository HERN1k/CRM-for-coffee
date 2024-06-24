using System.Security.Claims;
using System.Text;

using CRM.Application.RegEx;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.JwtToken;
using CRM.Core.Interfaces.PasswordHesher;
using CRM.Core.Interfaces.Repositories;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Options;

namespace CRM.Application.Services.AuthServices
{
  public class SignInService : ISignInService
  {
    private User? _user { get; set; }
    private readonly JwtSettings _jwtSettings;
    private readonly IRepository _repository;
    private readonly IHesherService _hashPassword;
    private readonly ITokenService _tokenServices;

    public SignInService(
        IOptions<JwtSettings> jwtSettings,
        IRepository repository,
        IHesherService hashPassword,
        ITokenService tokenServices
      )
    {
      _jwtSettings = jwtSettings.Value;
      _repository = repository;
      _hashPassword = hashPassword;
      _tokenServices = tokenServices;
    }

    public async Task SetData(string email)
    {
      var user = await _repository.FindSingleAsync<EntityUser>(e => e.Email == email)
        ?? throw new CustomException(ErrorTypes.InvalidOperationException, "The user is not registered");

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
      string date = _user.RegistrationDate.ToString("dd.MM.yyyy HH:mm:ss");
      string processedSalt = date.Replace(" ", "").Replace(".", "").Replace(":", "");
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
        tokenLifetime = _jwtSettings.AccessTokenLifetime;
      else if ((int)typesTokens == 2)
        tokenLifetime = _jwtSettings.RefreshTokenLifetime;

      return _tokenServices.CreateJwtToken(claims, tokenLifetime);
    }

    public async Task SaveToken(string token)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      bool tokenSaved = await _repository.AnyAsync<EntityRefreshToken>(e => e.Id == _user.Id);

      if (tokenSaved)
      {
        var updateToken = await _repository.FindSingleAsync<EntityRefreshToken>(e => e.Id == _user.Id)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

        updateToken.RefreshTokenString = token;

        await _repository.UpdateAsync<EntityRefreshToken>(updateToken);
        return;
      }

      var seveToken = new EntityRefreshToken
      {
        Id = _user.Id,
        RefreshTokenString = token
      };

      await _repository.AddAsync<EntityRefreshToken>(seveToken);
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
      if (typesTokens == TokenTypes.Access)
      {
        return new CookieOptions
        {
          MaxAge = TimeSpan.FromMinutes(_jwtSettings.AccessTokenLifetime),
        };
      }
      else if (typesTokens == TokenTypes.Refresh)
      {
        return new CookieOptions
        {
          MaxAge = TimeSpan.FromMinutes(_jwtSettings.RefreshTokenLifetime),
        };
      }
      else
      {
        return new CookieOptions
        {
          MaxAge = TimeSpan.FromMinutes(_jwtSettings.AccessTokenLifetime),
        };
      }
    }
  }
}