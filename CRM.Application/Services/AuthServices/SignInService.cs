using CRM.Application.RequestDataMapper;
using CRM.Application.RequestValidation;
using CRM.Application.Security;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.JwtToken;
using CRM.Core.Interfaces.Repositories;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services.AuthServices
{
  public class SignInService(
      IOptions<JwtSettings> jwtSettings,
      IRepository repository,
      ITokenService tokenServices
    ) : ISignInService
  {
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IRepository _repository = repository;
    private readonly ITokenService _tokenServices = tokenServices;

    public async Task<IActionResult> SignInAsync(HttpContext httpContext, SignInRequest request)
    {
      RequestValidator.Validate(request);

      var entityUser = await GetUserFromDB(request);

      User user = RequestMapper.MapToModel(entityUser);

      ChackingCorrectPassword(user, request);

      string accessToken = _tokenServices.GetJwtToken(user, TokenTypes.Access);
      string refreshToken = _tokenServices.GetJwtToken(user, TokenTypes.Refresh);

      await SaveToken(user, refreshToken);

      SetCookie(httpContext, accessToken, refreshToken);

      var response = CreateResponse(user, refreshToken);

      return new OkObjectResult(response);
    }

    private async Task<EntityUser> GetUserFromDB(SignInRequest request)
    {
      var result = await _repository.FindSingleAsync<EntityUser>(e => e.Email == request.Email)
        ?? throw new CustomException(ErrorTypes.InvalidOperationException, "The user is not registered");
      return result;
    }
    private void ChackingCorrectPassword(User user, SignInRequest request)
    {
      bool isCorrectPassword = HesherService.PasswordСheck(user, request.Password);
      if (!isCorrectPassword)
        throw new CustomException(ErrorTypes.BadRequest, "Incorrect password");
    }
    private async Task SaveToken(User user, string token)
    {
      bool tokenSaved = await _repository.AnyAsync<EntityRefreshToken>(e => e.Id == user.Id);

      if (tokenSaved)
      {
        var updateToken = await _repository.FindSingleAsync<EntityRefreshToken>(e => e.Id == user.Id)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");

        updateToken.RefreshTokenString = token;

        await _repository.UpdateAsync<EntityRefreshToken>(updateToken);
        return;
      }

      var seveToken = new EntityRefreshToken
      {
        Id = user.Id,
        RefreshTokenString = token
      };

      await _repository.AddAsync<EntityRefreshToken>(seveToken);
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
    private void SetCookie(HttpContext httpContext, string accessToken, string refreshToken)
    {
      httpContext.Response.Cookies.Append("accessToken", accessToken, SetCookieOptions(TokenTypes.Access));
      httpContext.Response.Cookies.Append("refreshToken", refreshToken, SetCookieOptions(TokenTypes.Refresh));
    }
    private SignInResponse CreateResponse(User user, string refreshToken)
    {
      return new SignInResponse
      {
        RefreshToken = refreshToken,
        FirstName = user.FirstName,
        LastName = user.LastName,
        FatherName = user.FatherName,
        Email = user.Email,
        Gender = user.Gender,
        Post = user.Post
      };
    }
  }
}