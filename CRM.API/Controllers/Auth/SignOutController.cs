using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.Controllers.Auth;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace CRM.API.Controllers.Auth
{
  [ApiController]
  [Route("Api/Auth")]
  public class SignOutController : ControllerBase, ISignOutController
  {
    private readonly ISignOutService _signOutService;

    public SignOutController(
        ISignOutService signOutService
      )
    {
      _signOutService = signOutService;
    }

    [SwaggerOperation(
      Summary = "Logs the user out.",
      OperationId = "Logout",
      Tags = ["Authorization"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(400, null, typeof(ExceptionResponse))]
    [SwaggerResponse(500, null, typeof(ExceptionResponse))]
    [HttpPost("SignOut")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
      string? token = HttpContext.Request.Cookies["accessToken"];
      if (string.IsNullOrEmpty(token))
        throw new CustomException(ErrorTypes.BadRequest, "Token not found");

      string email = _signOutService.TokenDecryption(token);

      await _signOutService.RemoveToken(email, token);

      HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });

      return Ok();
    }
  }
}