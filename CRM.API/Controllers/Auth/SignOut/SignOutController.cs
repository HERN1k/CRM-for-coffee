using CRM.Core.Interfaces.Services.AuthServices.SignOut;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace CRM.API.Controllers.Auth.SignOut
{
    [ApiController]
    [Route("api/auth")]
    public class SignOutController(
        ISignOutService signOutService
      ) : ControllerBase
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
        [HttpPost("sign_out")]
        [Authorize]
        public async Task<IActionResult> Logout() =>
          await _signOutService.LogoutAsync(HttpContext);
    }
}