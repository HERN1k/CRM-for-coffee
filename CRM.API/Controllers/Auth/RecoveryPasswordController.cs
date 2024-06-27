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
  public class RecoveryPasswordController(
      IAuthRecoveryService authRecoveryService
    ) : ControllerBase, IRecoveryPasswordController
  {
    private readonly IAuthRecoveryService _authRecoveryService = authRecoveryService;

    [SwaggerOperation(
      Summary = "Recovery user password.",
      OperationId = "RecoveryPassword",
      Tags = ["AuthSundry"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(400, null, typeof(ExceptionResponse))]
    [SwaggerResponse(500, null, typeof(ExceptionResponse))]
    [HttpPost("RecoveryPassword")]
    public async Task<IActionResult> RecoveryPassword(RecoveryPasswordRequest request) =>
      await _authRecoveryService.RecoveryPasswordAsync(HttpContext, request);
  }
}