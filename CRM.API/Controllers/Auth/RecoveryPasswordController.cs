using CRM.Core.Contracts.RestDto;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.Controllers.Auth;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace CRM.API.Controllers.Auth
{
  [ApiController]
  [Route("Api/Auth")]
  public class RecoveryPasswordController : ControllerBase, IRecoveryPasswordController
  {
    private readonly IAuthRecoveryService _authRecoveryService;

    public RecoveryPasswordController(
        IAuthRecoveryService authRecoveryService
      )
    {
      _authRecoveryService = authRecoveryService;
    }

    [SwaggerOperation(
      Summary = "Recovery user password.",
      OperationId = "RecoveryPassword",
      Tags = ["AuthSundry"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(400, null, typeof(ExceptionResponse))]
    [SwaggerResponse(500, null, typeof(ExceptionResponse))]
    [HttpPost("RecoveryPassword")]
    public async Task<IActionResult> RecoveryPassword(RecoveryPasswordRequest request)
    {
      _authRecoveryService.ValidationDataRecoveryPass(request);

      await _authRecoveryService.СomparisonRecoveryPassData(request);

      string newPassword = _authRecoveryService.GetNewPassword(16);

      await _authRecoveryService.SendRecoveryPassEmail(newPassword);

      await _authRecoveryService.SaveNewPassword(newPassword);

      await _authRecoveryService.RemoveRefreshToken();

      HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });

      return Ok();
    }
  }
}