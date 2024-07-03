using CRM.Core.Contracts.RestDto;
using CRM.Core.Interfaces.Services.AuthServices.AuthSundry;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace CRM.API.Controllers.Auth.AuthSundry
{
    [ApiController]
    [Route("api/auth")]
    public class AuthSundryController(
        IAuthSundryService authSundryService
      ) : ControllerBase
    {
        private readonly IAuthSundryService _authSundryService = authSundryService;

        [SwaggerOperation(
          Summary = "Update access token.",
          OperationId = "ChangeToken",
          Tags = ["AuthSundry"]
        )]
        [SwaggerResponse(200)]
        [SwaggerResponse(401)]
        [HttpPost("change_token")]
        public async Task<IActionResult> ChangeToken(ChangeTokenRequest request) =>
          await _authSundryService.ChangeTokenAsync(HttpContext, request);

        [SwaggerOperation(
          Summary = "Update user password.",
          OperationId = "UpdatePassword",
          Tags = ["AuthSundry"]
        )]
        [SwaggerResponse(200)]
        [SwaggerResponse(400, null, typeof(ExceptionResponse))]
        [SwaggerResponse(500, null, typeof(ExceptionResponse))]
        [HttpPost("update_password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest request) =>
          await _authSundryService.UpdatePasswordAsync(HttpContext, request);
    }
}