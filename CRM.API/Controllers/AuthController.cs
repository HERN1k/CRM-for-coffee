using System.Security.Claims;

using CRM.API.Types;
using CRM.Application.Types;
using CRM.Core.Contracts.RestDto;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
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

    [HttpGet("Test")]
    public IActionResult Test()
    {
      if (HttpContext.User.Identity == null)
        throw new CustomException(ErrorTypes.BadRequest, "Bad request");
      var isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
      var id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
      var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
      var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
      Console.WriteLine("-------------------------");
      Console.WriteLine($"IsAuthenticated - {isAuthenticated}");
      if (id != null && role != null && email != null)
      {
        Console.WriteLine($"Id - {id.Value}");
        Console.WriteLine($"Email - {email.Value}");
        Console.WriteLine($"Role - {role.Value}");
      }
      Console.WriteLine("-------------------------");
      return Ok();
    }

    #region [HttpPost("Register")]
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
      //  throw new CustomException(ErrorTypes.ValidationError, "You cannot register a user with administrator rights");

      _registerService.AddToModel(request);

      _registerService.ValidationDataRegister();

      await _registerService.UserIsRegister();

      _registerService.GetHash();

      await _registerService.SaveNewUser();

      //await _registerService.SendEmailConfirmRegister();

      return Ok();
    }
    #endregion

    #region [HttpGet("ConfirmRegister/{code}")]
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
      _registerService.FromBase64ToString(code);

      _registerService.ValidationEmail();

      await _registerService.ConfirmRegister();

      return Ok();
    }
    #endregion

    #region [HttpPost("SignIn")]
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
    #endregion

    #region [HttpPost("SignOut")]
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
        throw new CustomException(ErrorTypes.BadRequest, "Token not found");

      string email = _signOutService.TokenDecryption(token);

      await _signOutService.RemoveToken(email, token);

      HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });

      return Ok();
    }
    #endregion

    #region [HttpPost("ChangeToken")]
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
      try
      {
        string? refreshToken = HttpContext.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
          throw new Exception("Exception");

        _authSundryService.ValidationEmail(request.email);

        await _authSundryService.CheckImmutableToken(request.email, refreshToken);

        await _authSundryService.ValidateToken(refreshToken);

        string accessToken = _authSundryService.GetJwtAccessToken();

        HttpContext.Response.Cookies.Append("accessToken", accessToken, _authSundryService.SetCookieOptions());

        return Ok();
      }
      catch (Exception ex)
      {
        if (ex.Message == "Exception")
        {
          HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
          HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
          throw new CustomException(ErrorTypes.Unauthorized, "Unauthorized");
        }
        throw;
      }
    }
    #endregion

    #region [HttpPost("UpdatePassword")]
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
      _authSundryService.ValidateDataUpdatePassword(request);

      if (request.password == request.newPassword)
        throw new CustomException(ErrorTypes.BadRequest, "The old password is the same as the new one");

      await _authSundryService.SetDataUpdatePassword(request);

      _authSundryService.VerificationPassword(request.password);

      string newHash = _authSundryService.GetHash(request.newPassword);

      await _authSundryService.SaveNewPassword(newHash);

      await _authSundryService.RemoveRefreshToken();

      await _authSundryService.SendUpdatePasswordEmail();

      HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });

      return Ok();
    }
    #endregion

    #region [HttpPost("RecoveryPassword")]
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
      _authRecoveryService.ValidationDataRecoveryPass(request);

      await _authRecoveryService.СomparisonRecoveryPassData(request);

      string newPassword = _authRecoveryService.GetNewPassword(16);

      await _authRecoveryService.SendRecoveryPassEmail(newPassword);

      await _authRecoveryService.SaveNewPassword(newPassword);

      await _authRecoveryService.RemoveRefreshToken();

      HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });

      return Ok();
    }
    #endregion
  }
}