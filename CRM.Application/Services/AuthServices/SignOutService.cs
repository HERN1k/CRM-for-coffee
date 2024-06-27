using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.JwtToken;
using CRM.Core.Interfaces.Repositories;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Application.Services.AuthServices
{
  public class SignOutService(
      IRepository repository,
      ITokenService tokenService
    ) : ISignOutService
  {
    private readonly IRepository _repository = repository;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<IActionResult> LogoutAsync(HttpContext httpContext)
    {
      string token = GetAccessToken(httpContext);

      var claims = _tokenService.TokenDecryption(token);

      await RemoveToken(claims.Id);

      SetCookie(httpContext);

      return new OkResult();
    }

    private string GetAccessToken(HttpContext httpContext)
    {
      string? token = httpContext.Request.Cookies["accessToken"];
      if (string.IsNullOrEmpty(token))
        throw new CustomException(ErrorTypes.BadRequest, "Token not found");
      return token;
    }
    private async Task RemoveToken(string id)
    {
      bool isValid = Guid.TryParse(id, out Guid guid);
      if (!isValid)
        throw new CustomException(ErrorTypes.ServerError, "Server error");

      await _repository.RemoveAsync<EntityRefreshToken>(e => e.Id == guid);
    }
    private void SetCookie(HttpContext httpContext)
    {
      httpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      httpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
    }
  }
}