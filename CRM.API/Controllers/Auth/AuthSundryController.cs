using CRM.Core.Contracts.RestDto;
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
  public class AuthSundryController : ControllerBase, IAuthSundryController
  {
    private readonly IAuthSundryService _authSundryService;

    public AuthSundryController(
        IAuthSundryService authSundryService
      )
    {
      _authSundryService = authSundryService;
    }

    [SwaggerOperation(
      Summary = "Update access token.",
      OperationId = "ChangeToken",
      Tags = ["AuthSundry"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(401)]
    [HttpPost("ChangeToken")]
    public async Task<IActionResult> ChangeToken(ChangeTokenRequest request)
    {
      try
      {
        string? refreshToken = HttpContext.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
          throw new CustomException(ErrorTypes.BadRequest, "Token not found");

        _authSundryService.ValidationEmail(request.email);

        await _authSundryService.ValidateToken(refreshToken);

        await _authSundryService.CheckImmutableToken(request.email, refreshToken);

        string accessToken = _authSundryService.GetJwtAccessToken();

        HttpContext.Response.Cookies.Append("accessToken", accessToken, _authSundryService.SetCookieOptions());

        return Ok();
      }
      catch (CustomException ex)
      {
        HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
        HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
        throw new CustomException(ErrorTypes.Unauthorized, ex.Message);
      }
      catch (Exception ex)
      {
        HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
        HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
        throw new CustomException(ErrorTypes.Unauthorized, ex.Message);
      }
    }

    [SwaggerOperation(
      Summary = "Update user password.",
      OperationId = "UpdatePassword",
      Tags = ["AuthSundry"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(400, null, typeof(ExceptionResponse))]
    [SwaggerResponse(500, null, typeof(ExceptionResponse))]
    [HttpPost("UpdatePassword")]
    [Authorize]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest request)
    {
      await _authSundryService.ValidateDataUpdatePassword(request);

      if (request.password == request.newPassword)
        throw new CustomException(ErrorTypes.BadRequest, "The old password is the same as the new one");

      _authSundryService.VerificationPassword(request.password);

      string newHash = _authSundryService.GetHash(request.newPassword);

      await _authSundryService.SaveNewPassword(newHash);

      await _authSundryService.RemoveRefreshToken();

      await _authSundryService.SendUpdatePasswordEmail();

      HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });

      return Ok();
    }
  }
}