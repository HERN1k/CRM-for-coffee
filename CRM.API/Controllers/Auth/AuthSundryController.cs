using CRM.Core.Contracts.RestDto;
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
  public class AuthSundryController(
      IAuthSundryService authSundryService
    ) : ControllerBase, IAuthSundryController
  {
    private readonly IAuthSundryService _authSundryService = authSundryService;

    [SwaggerOperation(
      Summary = "Update access token.",
      OperationId = "ChangeToken",
      Tags = ["AuthSundry"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(401)]
    [HttpPost("ChangeToken")]
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
    [HttpPost("UpdatePassword")]
    [Authorize]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest request) =>
      await _authSundryService.UpdatePasswordAsync(HttpContext, request);
  }
}