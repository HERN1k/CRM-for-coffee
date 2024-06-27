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
  public class SignInController(
      ISignInService signInService
    ) : ControllerBase, ISignInController
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
    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn(SignInRequest request) =>
      await _signInService.SignInAsync(HttpContext, request);
  }
}