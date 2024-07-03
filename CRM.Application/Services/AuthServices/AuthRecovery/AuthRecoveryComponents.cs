using System.Text.RegularExpressions;

using CRM.Application.Tools.RegEx;
using CRM.Application.Tools.Security;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Services.AuthServices.AuthRecovery;
using CRM.Core.Models;

using Microsoft.AspNetCore.Http;

namespace CRM.Application.Services.AuthServices.AuthRecovery
{
  public class AuthRecoveryComponents : IAuthRecoveryComponents
  {
    public void СomparisonRecoveryPassData(User user, RecoveryPasswordRequest request)
    {
      if (user.PhoneNumber != request.PhoneNumber)
        throw new CustomException(ErrorTypes.BadRequest, "Some data doesn't match");
      if (user.Post != request.Post)
        throw new CustomException(ErrorTypes.BadRequest, "Some data doesn't match");
    }

    public string GetNewPassword(int length)
    {
      string newPassword;
      bool isCorrect;
      int iterations = 0;
      do
      {
        newPassword = HesherService.GetRandomPassword(length);
        isCorrect = Regex.IsMatch(newPassword, RegExPatterns.Password);
        iterations++;
      } while (!(isCorrect || iterations >= 10));

      if (string.IsNullOrEmpty(newPassword) || iterations >= 10)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      return newPassword;
    }

    public void SetUnauthorizedCookies(HttpContext httpContext)
    {
      httpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      httpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
    }
  }
}