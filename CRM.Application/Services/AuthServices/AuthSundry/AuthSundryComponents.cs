using CRM.Application.Tools.Security;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Services.AuthServices.AuthSundry;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CRM.Application.Services.AuthServices.AuthSundry
{
  public class AuthSundryComponents(
      IOptions<JwtSettings> jwtSettings
    ) : IAuthSundryComponents
  {
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public string GetRefreshToken(HttpContext httpContext)
    {
      string? token = httpContext.Request.Cookies["refreshToken"];
      if (string.IsNullOrEmpty(token))
        throw new CustomException(ErrorTypes.BadRequest, "Token not found");

      return token;
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

    public void SetAccessTokenCookie(HttpContext httpContext, string accessToken)
    {
      if (string.IsNullOrEmpty(accessToken))
        throw new ArgumentNullException(nameof(accessToken));

      httpContext.Response.Cookies.Append("accessToken", accessToken, SetCookieOptions(TokenTypes.Access));
    }

    public void SetUnauthorizedCookies(HttpContext httpContext)
    {
      httpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      httpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
    }

    public void CheckPasswordDifference(UpdatePasswordRequest request)
    {
      if (request.Password == request.NewPassword)
        throw new CustomException(ErrorTypes.BadRequest, "The old password is the same as the new one");
    }

    public void PasswordСheck(User user, string requestPassword)
    {
      if (string.IsNullOrEmpty(requestPassword))
        throw new ArgumentNullException(nameof(requestPassword));

      var result = HesherService.PasswordСheck(user, requestPassword);
      if (!result)
        throw new CustomException(ErrorTypes.BadRequest, "Incorrect password");
    }
  }
}