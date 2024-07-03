using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Services.AuthServices.SignOut;

using Microsoft.AspNetCore.Http;

namespace CRM.Application.Services.AuthServices.SignOut
{
  public class SignOutComponents : ISignOutComponents
  {
    public string GetAccessToken(HttpContext httpContext)
    {
      string? token = httpContext.Request.Cookies["accessToken"];
      if (string.IsNullOrEmpty(token))
        throw new CustomException(ErrorTypes.BadRequest, "Token not found");
      return token;
    }

    public void SetCookie(HttpContext httpContext)
    {
      httpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      httpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
    }
  }
}