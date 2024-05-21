using CRM.API.Contarcts;
using CRM.API.Types;
using CRM.Application.Security;
using CRM.Application.Types;
using CRM.Core.Contracts;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace CRM.API.Controllers
{
  [ApiController]
  [Route("Api/[controller]")]
  public class AuthController : ControllerBase, IAuthController
  {
    private readonly IRegisterService _registerService;
    private readonly ISignInService _signInService;
    private readonly ISignOutService _signOutService;
    private readonly IAuthSundryService _authSundryService;
    private readonly IAuthRecoveryService _authRecoveryService;

    public AuthController(
        IRegisterService registerServices,
        ISignInService signInService,
        ISignOutService signOutService,
        IAuthSundryService authSundryService,
        IAuthRecoveryService authRecoveryService
      )
    {
      _registerService = registerServices;
      _signInService = signInService;
      _signOutService = signOutService;
      _authSundryService = authSundryService;
      _authRecoveryService = authRecoveryService;
    }

    [SwaggerOperation(
      Summary = "Registers a new user.",
      OperationId = "Register",
      Tags = ["Registration"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(400, "The user has already been registered.", typeof(ErrorsResponse))]
    [SwaggerResponse(500, "Server error.", typeof(ErrorsResponse))]
    [HttpPost("Register")]
    //[Authorize(Policy = "ManagerOrUpper")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
      //if (request.post == "Admin")
      //  return BadRequest(new ErrorsResponse(500, "You cannot register a user with administrator rights."));

      bool setUser = _registerService.AddToModel(request);
      if (!setUser)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      var isValidData = _registerService.ValidationDataRegister();
      if (!isValidData.IsSuccess)
        return BadRequest(new ErrorsResponse(400, $"{isValidData.Field} is incorrect or null."));

      bool userIsRegister = await _registerService.UserIsRegister();
      if (userIsRegister)
        return BadRequest(new ErrorsResponse(400, "The user has already been registered."));

      bool hashPassword = _registerService.GetHash();
      if (!hashPassword)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      bool setNewId = await _registerService.SetNewId();
      if (!setNewId)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      var saveUser = await _registerService.SaveNewUser();
      if (!saveUser)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      //bool sendEmail = await _registerService.SendEmailConfirmRegister();
      //if (!sendEmail)
      //{
      //  await _registerService.RemoveUser();
      //  return BadRequest(new ErrorsResponse(500, "Server error."));
      //}

      return Ok();
    }

    [SwaggerOperation(
      Summary = "Confirmation of registration.",
      OperationId = "ConfirmRegister",
      Tags = ["Registration"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(400, "Code is invalid.", typeof(ErrorsResponse))]
    [SwaggerResponse(500, "Server error.", typeof(ErrorsResponse))]
    [HttpGet("ConfirmRegister/{code}")]
    public async Task<IActionResult> ConfirmRegister(string code)
    {
      bool converted = _registerService.FromBase64ToString(code);
      if (!converted)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      bool isValidData = _registerService.ValidationEmail();
      if (!isValidData)
        return BadRequest(new ErrorsResponse(400, "Code is invalid."));

      bool result = await _registerService.ConfirmRegister();
      if (!result)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      return Ok();
    }

    [SwaggerOperation(
      Summary = "User authorization.",
      OperationId = "SignIn",
      Tags = ["Authorization"]
    )]
    [SwaggerResponse(200, "OK", typeof(SignInResponse))]
    [SwaggerResponse(400, "The user is not registered.", typeof(ErrorsResponse))]
    [SwaggerResponse(500, "Server error.", typeof(ErrorsResponse))]
    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn(SignInRequest request)
    {
      var isValidData = _signInService.ValidationDataSignIn(request);
      if (!isValidData.IsSuccess)
        return BadRequest(new ErrorsResponse(400, $"{isValidData.Field} is incorrect or null."));

      bool setData = await _signInService.SetData(request.email);
      if (!setData)
        return BadRequest(new ErrorsResponse(400, "The user did not confirm the email."));

      bool verifiPassword = _signInService.VerificationHash(request.password);
      if (!verifiPassword)
        return BadRequest(new ErrorsResponse(400, "Incorrect password."));

      string accessToken = _signInService.GetJwtToken(TypesToken.Access);
      string refreshToken = _signInService.GetJwtToken(TypesToken.Refresh);

      bool saveToken = await _signInService.SaveToken(refreshToken);
      if (!saveToken)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      var response = _signInService.SetResponse(refreshToken);
      if (response == null)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      HttpContext.Response.Cookies.Append("accessToken", accessToken, _signInService.SetCookieOptions());

      return Ok(response);
    }

    [SwaggerOperation(
      Summary = "Logs the user out.",
      OperationId = "Logout",
      Tags = ["Authorization"]
    )]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(400, "Token not found.", typeof(ErrorsResponse))]
    [SwaggerResponse(500, "Server error.", typeof(ErrorsResponse))]
    [HttpPost("SignOut")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
      string? token = HttpContext.Request.Cookies["accessToken"];
      if (string.IsNullOrEmpty(token))
        return BadRequest(new ErrorsResponse(400, "Token not found."));

      string email = _signOutService.TokenDecryption(token);
      if (string.IsNullOrEmpty(email))
        return BadRequest(new ErrorsResponse(400, "Token not found."));

      bool remove = await _signOutService.RemoveToken(email, token);
      if (!remove)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });

      return Ok();
    }

    [SwaggerOperation(
      Summary = "Update access token.",
      OperationId = "ChangeToken",
      Tags = ["AuthSundry"]
    )]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(401, "Unauthorized")]
    [HttpPost("ChangeToken")]
    public async Task<IActionResult> ChangeToken(ChangeTokenRequest request)
    {
      bool isValidData = _authSundryService.ValidationEmail(request.email);
      if (!isValidData)
        return Unauthorized();

      bool isImmutableToken = await _authSundryService.CheckImmutableToken(request.email, request.refreshToken);
      if (!isImmutableToken)
        return Unauthorized();

      bool tokenIsValid = await _authSundryService.ValidateToken(request.refreshToken);
      if (!tokenIsValid)
      {
        await _authSundryService.RemoveRefreshToken();
        return Unauthorized();
      }

      string accessToken = _authSundryService.GetJwtAccessToken();
      if (string.IsNullOrEmpty(accessToken))
        return Unauthorized();

      HttpContext.Response.Cookies.Append("accessToken", accessToken, _authSundryService.SetCookieOptions());

      return Ok();
    }

    [SwaggerOperation(
      Summary = "Update user password.",
      OperationId = "UpdatePassword",
      Tags = ["AuthSundry"]
    )]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(400, "Incorrect password.", typeof(ErrorsResponse))]
    [SwaggerResponse(500, "Server error.", typeof(ErrorsResponse))]
    [HttpPost("UpdatePassword")]
    [Authorize]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest request)
    {
      var isValidData = _authSundryService.ValidateDataUpdatePassword(request);
      if (!isValidData.IsSuccess)
        return BadRequest(new ErrorsResponse(400, $"{isValidData.Field} is incorrect or null."));

      if (request.password == request.newPassword)
        return BadRequest(new ErrorsResponse(400, $"The old password is the same as the new one."));

      bool setData = await _authSundryService.SetDataUpdatePassword(request);
      if (!setData)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      bool verifiPassword = _authSundryService.VerificationPassword(request.password);
      if (!verifiPassword)
        return BadRequest(new ErrorsResponse(400, "Incorrect password."));

      string newHash = _authSundryService.GetHash(request.newPassword);
      if (string.IsNullOrEmpty(newHash))
        return BadRequest(new ErrorsResponse(500, "Server error."));

      bool success = await _authSundryService.SaveNewPassword(newHash);
      if (!success)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      bool isRemoved = await _authSundryService.RemoveRefreshToken();
      if (!isRemoved)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      bool sendEmail = await _authSundryService.SendUpdatePasswordEmail();
      if (!sendEmail)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });

      return Ok();
    }

    [SwaggerOperation(
      Summary = "Recovery user password.",
      OperationId = "RecoveryPassword",
      Tags = ["AuthSundry"]
    )]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(400, "Some data doesn't match.", typeof(ErrorsResponse))]
    [SwaggerResponse(500, "Server error.", typeof(ErrorsResponse))]
    [HttpPost("RecoveryPassword")]
    public async Task<IActionResult> RecoveryPassword(RecoveryPasswordRequest request)
    {
      var isValidData = _authRecoveryService.ValidationDataRcoveryPass(request);
      if (!isValidData.IsSuccess)
        return BadRequest(new ErrorsResponse(400, $"{isValidData.Field} is incorrect or null."));

      bool comparison = await _authRecoveryService.СomparisonRecoveryPassData();
      if (!comparison)
        return BadRequest(new ErrorsResponse(400, "Some data doesn't match."));

      string newPassword = _authRecoveryService.GetNewPassword(16);
      if (string.IsNullOrEmpty(newPassword))
        return BadRequest(new ErrorsResponse(500, "Server error."));

      bool sendEmail = await _authRecoveryService.SendRecoveryPassEmail(newPassword);
      if (!sendEmail)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      bool savePass = await _authRecoveryService.SaveNewPassword(newPassword);
      if (!savePass)
        return BadRequest(new ErrorsResponse(500, "Server error."));

      return Ok();
    }
  }
}