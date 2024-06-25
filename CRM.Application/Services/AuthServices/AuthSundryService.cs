using System.Security.Claims;
using System.Text;

using CRM.Application.RegEx;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.Email;
using CRM.Core.Interfaces.JwtToken;
using CRM.Core.Interfaces.PasswordHesher;
using CRM.Core.Interfaces.Repositories;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services.AuthServices
{
  public class AuthSundryService(
      IOptions<JwtSettings> jwtSettings,
      IRepository repository,
      ITokenService tokenService,
      IHesherService hesherService,
      IEmailService emailService
    ) : IAuthSundryService
  {
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IRepository _repository = repository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IHesherService _hesherService = hesherService;
    private readonly IEmailService _emailService = emailService;
    private User? _user { get; set; }

    public void ValidationEmail(string email)
    {
      bool result = RegExHelper.ChackString(email, RegExPatterns.Email);
      if (!result)
        throw new CustomException(ErrorTypes.Unauthorized, "Unauthorized");
    }

    public async Task CheckImmutableToken(string email, string refreshToken)
    {
      var user = await _repository.FindSingleAsync<EntityUser>(e => e.Email == email)
        ?? throw new CustomException(ErrorTypes.Unauthorized, "Unauthorized");

      _user = new User
      {
        Id = user.Id,
        FirstName = user.FirstName,
        Email = user.Email,
        Post = user.Post,
        Age = user.Age,
        FatherName = user.FatherName,
        Gender = user.Gender,
        IsConfirmed = user.IsConfirmed,
        LastName = user.LastName,
        Password = user.Password,
        PhoneNumber = user.PhoneNumber,
        RegistrationDate = user.RegistrationDate
      };

      var token = await _repository.FindSingleAsync<EntityRefreshToken>(e => e.Id == _user.Id)
        ?? throw new CustomException(ErrorTypes.Unauthorized, "Unauthorized");

      if (token.RefreshTokenString != refreshToken)
        throw new CustomException(ErrorTypes.Unauthorized, "Unauthorized");
    }

    public async Task ValidateToken(string token) =>
      await _tokenService.ValidateToken(token);

    public async Task RemoveRefreshToken()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      await _repository.RemoveAsync<EntityRefreshToken>(e => e.Id == _user.Id);
    }

    public string GetJwtAccessToken()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      int tokenLifetime = _jwtSettings.AccessTokenLifetime;

      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.NameIdentifier, _user.Id.ToString()),
        new Claim(ClaimTypes.Email, _user.Email),
        new Claim(ClaimTypes.Role, _user.Post),
      };

      string result = _tokenService.CreateJwtToken(claims, tokenLifetime);
      if (string.IsNullOrEmpty(result))
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      return result;
    }

    public async Task ValidateDataUpdatePassword(UpdatePasswordRequest request)
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

      var user = await _repository.FindSingleAsync<EntityUser>(e => e.Email == request.email)
        ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      _user = new User
      {
        Id = user.Id,
        FirstName = user.FirstName,
        Email = user.Email,
        Post = user.Post,
        Age = user.Age,
        FatherName = user.FatherName,
        Gender = user.Gender,
        IsConfirmed = user.IsConfirmed,
        LastName = user.LastName,
        Password = user.Password,
        PhoneNumber = user.PhoneNumber,
        RegistrationDate = user.RegistrationDate
      };
    }

    public void VerificationPassword(string input)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      string date = _user.RegistrationDate.ToString("dd.MM.yyyy HH:mm:ss");
      string processedSalt = date.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string hash = _hesherService.GetHash(input, saltArray);
      bool result = hash == _user.Password;
      if (!result)
        throw new CustomException(ErrorTypes.BadRequest, "Incorrect password");
    }

    public CookieOptions SetCookieOptions()
    {
      return new CookieOptions
      {
        MaxAge = TimeSpan.FromMinutes(_jwtSettings.AccessTokenLifetime),
      };
    }

    public string GetHash(string input)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      string date = _user.RegistrationDate.ToString("dd.MM.yyyy HH:mm:ss");
      string processedSalt = date.Replace(" ", "").Replace(".", "").Replace(":", "");
      byte[] saltArray = Encoding.Default.GetBytes(processedSalt);
      string result = _hesherService.GetHash(input, saltArray);
      if (string.IsNullOrEmpty(result))
        throw new CustomException(ErrorTypes.ServerError, "Server error");
      return result;
    }

    public async Task SaveNewPassword(string input)
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      var user = await _repository.FindSingleAsync<EntityUser>(e => e.Id == _user.Id)
        ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      user.Password = input;

      await _repository.UpdateAsync<EntityUser>(user);
    }

    public async Task SendUpdatePasswordEmail()
    {
      if (_user == null)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      string html = await _emailService.CompileHtmlStringAsync("UpdatePassword", new { });

      await _emailService.SendEmailAsync(_user.FirstName, _user.Email, html);
    }
  }
}