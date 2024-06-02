using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.Controllers.Auth;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace CRM.API.Controllers.Auth
{
  [ApiController]
  [Route("Api/Auth")]
  public class SignInController : ControllerBase, ISignInController
  {
    private readonly ISignInService _signInService;

    public SignInController(
        ISignInService signInService
      )
    {
      _signInService = signInService;
    }

    [SwaggerOperation(
      Summary = "User authorization.",
      OperationId = "SignIn",
      Tags = ["Authorization"]
    )]
    [SwaggerResponse(200, null, typeof(SignInResponse))]
    [SwaggerResponse(400, null, typeof(ExceptionResponse))]
    [SwaggerResponse(500, null, typeof(ExceptionResponse))]
    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn(SignInRequest request)
    {
      _signInService.ValidationDataSignIn(request);

      await _signInService.SetData(request.email);

      _signInService.VerificationHash(request.password);

      string accessToken = _signInService.GetJwtToken(TokenTypes.Access);
      string refreshToken = _signInService.GetJwtToken(TokenTypes.Refresh);

      await _signInService.SaveToken(refreshToken);

      var response = _signInService.SetResponse(refreshToken);

      HttpContext.Response.Cookies.Append("accessToken", accessToken, _signInService.SetCookieOptions(TokenTypes.Access));
      HttpContext.Response.Cookies.Append("refreshToken", refreshToken, _signInService.SetCookieOptions(TokenTypes.Refresh));

      return Ok(response);
    }
  }
}