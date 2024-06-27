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
  public class SignOutController(
      ISignOutService signOutService
    ) : ControllerBase, ISignOutController
  {
    private readonly ISignOutService _signOutService = signOutService;

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
    public async Task<IActionResult> Logout() =>
      await _signOutService.LogoutAsync(HttpContext);
  }
}