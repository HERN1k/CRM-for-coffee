using CRM.Core.Contracts.RestDto;
using CRM.Core.Interfaces.Services.AuthServices.SignIn;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace CRM.API.Controllers.Auth.SignIn
{
    [ApiController]
    [Route("api/auth")]
    public class SignInController(
        ISignInService signInService
      ) : ControllerBase
    {
        private readonly ISignInService _signInService = signInService;

        [SwaggerOperation(
          Summary = "User authorization.",
          OperationId = "SignIn",
          Tags = ["Authorization"]
        )]
        [SwaggerResponse(200, null, typeof(SignInResponse))]
        [SwaggerResponse(400, null, typeof(ExceptionResponse))]
        [SwaggerResponse(500, null, typeof(ExceptionResponse))]
        [HttpPost("sign_in")]
        public async Task<IActionResult> SignIn(SignInRequest request) =>
          await _signInService.SignInAsync(HttpContext, request);
    }
}