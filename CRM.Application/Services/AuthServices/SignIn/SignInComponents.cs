using CRM.Application.Tools.Security;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Services.AuthServices.SignIn;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Interfaces.Tools.Security.JwtToken;
using CRM.Core.Models;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services.AuthServices.SignIn
{
  public class SignInComponents(
      IOptions<JwtSettings> jwtSettings,
      ITokenService tokenServices
    ) : ISignInComponents
  {
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly ITokenService _tokenServices = tokenServices;

    public void ChackingCorrectPassword(User user, SignInRequest request)
    {
      bool isCorrectPassword = HesherService.PasswordСheck(user, request.Password);
      if (!isCorrectPassword)
        throw new CustomException(ErrorTypes.BadRequest, "Incorrect password");
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

    public void SetCookie(HttpContext httpContext, string accessToken, string refreshToken)
    {
      httpContext.Response.Cookies.Append("accessToken", accessToken, SetCookieOptions(TokenTypes.Access));
      httpContext.Response.Cookies.Append("refreshToken", refreshToken, SetCookieOptions(TokenTypes.Refresh));
    }

    public Dictionary<TokenTypes, string> CreateJwtTokenDictionary(User user)
    {
      string accessToken = _tokenServices.GetJwtToken(user, TokenTypes.Access);
      string refreshToken = _tokenServices.GetJwtToken(user, TokenTypes.Refresh);

      var response = new Dictionary<TokenTypes, string>
      {
        { TokenTypes.Access, accessToken },
        { TokenTypes.Refresh, refreshToken }
      };

      return response;
    }

    public SignInResponse CreateResponse(User user, string refreshToken)
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