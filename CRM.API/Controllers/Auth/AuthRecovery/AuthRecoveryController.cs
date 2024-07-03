using CRM.Core.Contracts.RestDto;
using CRM.Core.Interfaces.Services.AuthServices.AuthRecovery;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace CRM.API.Controllers.Auth.AuthRecovery
{
    [ApiController]
    [Route("api/auth")]
    public class AuthRecoveryController(
        IAuthRecoveryService authRecoveryService
      ) : ControllerBase
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
        [HttpPost("recovery_password")]
        public async Task<IActionResult> RecoveryPassword(RecoveryPasswordRequest request) =>
          await _authRecoveryService.RecoveryPasswordAsync(HttpContext, request);
    }
}