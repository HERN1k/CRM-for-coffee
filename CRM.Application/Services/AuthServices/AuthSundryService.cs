using CRM.Application.RequestDataMapper;
using CRM.Application.RequestValidation;
using CRM.Application.Security;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.Email;
using CRM.Core.Interfaces.JwtToken;
using CRM.Core.Interfaces.Repositories;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services.AuthServices
{
  public class AuthSundryService(
      IOptions<JwtSettings> jwtSettings,
      IRepository repository,
      ITokenService tokenService,
      IEmailSender emailSender
    ) : IAuthSundryService
  {
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IRepository _repository = repository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IEmailSender _emailSender = emailSender;

    public async Task<IActionResult> ChangeTokenAsync(HttpContext httpContext, ChangeTokenRequest request)
    {
      try
      {
        string token = GetRefreshToken(httpContext);

        RequestValidator.ValidateEmail(request.Email);

        await _tokenService.ValidateToken(token);

        EntityUser entityUser = await FindUserFromDB(request.Email, ErrorTypes.Unauthorized, "Unauthorized");

        User user = RequestMapper.MapToModel(entityUser);

        await CheckImmutableToken(user, token);

        string accessToken = _tokenService.GetJwtToken(user, TokenTypes.Access);

        SetAccessTokenCookie(httpContext, accessToken);

        return new OkResult();
      }
      catch (CustomException ex)
      {
        SetUnauthorizedCookies(httpContext);
        throw new CustomException(ErrorTypes.Unauthorized, ex.Message);
      }
      catch (Exception ex)
      {
        SetUnauthorizedCookies(httpContext);
        throw new CustomException(ErrorTypes.Unauthorized, ex.Message);
      }
    }
    public async Task<IActionResult> UpdatePasswordAsync(HttpContext httpContext, UpdatePasswordRequest request)
    {
      RequestValidator.Validate(request);

      EntityUser entityUser = await FindUserFromDB(request.Email, ErrorTypes.ServerError, "Server error");

      User user = RequestMapper.MapToModel(entityUser);

      CheckPasswordDifference(request);

      PasswordСheck(user, request.Password);

      string hash = HesherService.GetPasswordHash(user, request.Password);

      await SaveNewPassword(user, hash);

      await RemoveRefreshToken(user);

      await _emailSender.SendUpdatePasswordEmail(user);

      SetUnauthorizedCookies(httpContext);

      return new OkResult();
    }

    private string GetRefreshToken(HttpContext httpContext)
    {
      string? token = httpContext.Request.Cookies["refreshToken"];
      if (string.IsNullOrEmpty(token))
        throw new CustomException(ErrorTypes.BadRequest, "Token not found");
      return token;
    }
    private async Task<EntityUser> FindUserFromDB(string email, ErrorTypes type, string errorMassage)
    {
      var result = await _repository
        .FindSingleAsync<EntityUser>(e => e.Email == email)
          ?? throw new CustomException(type, errorMassage);
      return result;
    }
    private async Task CheckImmutableToken(User user, string refreshToken)
    {
      var token = await _repository
        .FindSingleAsync<EntityRefreshToken>(e => e.Id == user.Id)
          ?? throw new CustomException(ErrorTypes.Unauthorized, "Unauthorized");

      if (token.RefreshTokenString != refreshToken)
        throw new CustomException(ErrorTypes.Unauthorized, "Unauthorized");
    }
    private CookieOptions SetCookieOptions(TokenTypes typesTokens)
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
    private void SetAccessTokenCookie(HttpContext httpContext, string accessToken)
    {
      httpContext.Response.Cookies.Append("accessToken", accessToken, SetCookieOptions(TokenTypes.Access));
    }
    private void SetUnauthorizedCookies(HttpContext httpContext)
    {
      httpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      httpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
    }
    private void CheckPasswordDifference(UpdatePasswordRequest request)
    {
      if (request.Password == request.NewPassword)
        throw new CustomException(ErrorTypes.BadRequest, "The old password is the same as the new one");
    }
    private void PasswordСheck(User user, string requestPassword)
    {
      var result = HesherService.PasswordСheck(user, requestPassword);
      if (!result)
        throw new CustomException(ErrorTypes.BadRequest, "Incorrect password");
    }
    private async Task SaveNewPassword(User user, string hash)
    {
      var entityUser = await _repository
        .FindSingleAsync<EntityUser>(e => e.Id == user.Id)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

      entityUser.Password = hash;

      await _repository.UpdateAsync<EntityUser>(entityUser);
    }
    private async Task RemoveRefreshToken(User user)
    {
      await _repository.RemoveAsync<EntityRefreshToken>(e => e.Id == user.Id);
    }
  }
}