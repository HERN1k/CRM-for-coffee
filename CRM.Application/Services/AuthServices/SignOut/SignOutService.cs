using CRM.Core.Interfaces.Repositories.AuthRepositories.SignOut;
using CRM.Core.Interfaces.Services.AuthServices.SignOut;
using CRM.Core.Interfaces.Tools.Security.JwtToken;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Application.Services.AuthServices.SignOut
{
  public class SignOutService(
      ISignOutRepository repository,
      ISignOutComponents components,
      ITokenService tokenService
    ) : ISignOutService
  {
    private readonly ISignOutRepository _repository = repository;
    private readonly ISignOutComponents _components = components;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<IActionResult> LogoutAsync(HttpContext httpContext)
    {
      string token = _components.GetAccessToken(httpContext);

      var claims = _tokenService.TokenDecryption(token);

      await _repository.RemoveToken(claims.Id);

      _components.SetCookie(httpContext);

      return new OkResult();
    }
  }
}