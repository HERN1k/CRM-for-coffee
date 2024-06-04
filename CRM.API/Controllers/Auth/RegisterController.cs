using System.Security.Claims;

using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.Controllers.Auth;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace CRM.API.Controllers.Auth
{
  [ApiController]
  [Route("Api/Auth")]
  public class RegisterController : ControllerBase, IRegisterController
  {
    private readonly IRegisterService _registerService;

    public RegisterController(
        IRegisterService registerServices
      )
    {
      _registerService = registerServices;
    }

    [HttpGet("Test")]
    public IActionResult Test()
    {
      if (HttpContext.User.Identity == null)
        throw new CustomException(ErrorTypes.BadRequest, "Bad request");

      if (!HttpContext.User.Identity.IsAuthenticated)
        throw new CustomException(ErrorTypes.Unauthorized, "Unauthorized");

      string role = HttpContext.User.Claims
        .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;

      return Ok(new { Post = role });
    }

    [SwaggerOperation(
      Summary = "Registers a new user.",
      OperationId = "Register",
      Tags = ["Registration"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(400, null, typeof(ExceptionResponse))]
    [SwaggerResponse(500, null, typeof(ExceptionResponse))]
    [HttpPost("Register")]
    //[Authorize(Policy = "ManagerOrUpper")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
      //if (request.post == "Admin")
      //  throw new CustomException(ErrorTypes.ValidationError, "You cannot register a user with administrator rights");

      _registerService.AddToModel(request);

      _registerService.ValidationDataRegister();

      await _registerService.UserIsRegister();

      _registerService.GetHash();

      //await _registerService.SendEmailConfirmRegister();

      await _registerService.SaveNewUser(); // внутри важно удалить!

      return Ok();
    }

    [SwaggerOperation(
      Summary = "Confirmation of registration.",
      OperationId = "ConfirmRegister",
      Tags = ["Registration"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(400, null, typeof(ExceptionResponse))]
    [SwaggerResponse(500, null, typeof(ExceptionResponse))]
    [HttpGet("ConfirmRegister/{code}")]
    public async Task<IActionResult> ConfirmRegister(string code)
    {
      _registerService.FromBase64ToString(code);

      _registerService.ValidationEmail();

      await _registerService.ConfirmRegister();

      return Ok();
    }
  }
}