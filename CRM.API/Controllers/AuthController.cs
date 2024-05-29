using System.Security.Claims;

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

    [HttpGet("Test")]
    public async Task<IActionResult> Test()
    {
      if (HttpContext.User.Identity == null)
        return BadRequest();
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
        return Ok(new ErrorsResponse(500, "Server error."));

      var isValidData = _registerService.ValidationDataRegister();
      if (!isValidData.IsSuccess)
        return Ok(new ErrorsResponse(400, $"{isValidData.Field} is incorrect or null."));

      bool userIsRegister = await _registerService.UserIsRegister();
      if (userIsRegister)
        return Ok(new ErrorsResponse(400, "The user has already been registered."));

      bool hashPassword = _registerService.GetHash();
      if (!hashPassword)
        return Ok(new ErrorsResponse(500, "Server error."));

      var saveUser = await _registerService.SaveNewUser();
      if (!saveUser)
        return Ok(new ErrorsResponse(500, "Server error."));

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
        return Ok(new ErrorsResponse(500, "Server error."));

      bool isValidData = _registerService.ValidationEmail();
      if (!isValidData)
        return Ok(new ErrorsResponse(400, "Code is invalid."));

      bool result = await _registerService.ConfirmRegister();
      if (!result)
        return Ok(new ErrorsResponse(500, "Server error."));

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
        return Ok(new ErrorsResponse(400, $"{isValidData.Field} is incorrect or null."));

      bool setData = await _signInService.SetData(request.email);
      if (!setData)
        return Ok(new ErrorsResponse(400, "The user is not registered or has not confirmed the email."));

      bool verifiPassword = _signInService.VerificationHash(request.password);
      if (!verifiPassword)
        return Ok(new ErrorsResponse(400, "Incorrect password."));

      string accessToken = _signInService.GetJwtToken(TypesToken.Access);
      string refreshToken = _signInService.GetJwtToken(TypesToken.Refresh);

      bool saveToken = await _signInService.SaveToken(refreshToken);
      if (!saveToken)
        return Ok(new ErrorsResponse(500, "Server error."));

      var response = _signInService.SetResponse(refreshToken);
      if (response == null)
        return Ok(new ErrorsResponse(500, "Server error."));

      HttpContext.Response.Cookies.Append("accessToken", accessToken, _signInService.SetCookieOptions(TypesToken.Access));
      HttpContext.Response.Cookies.Append("refreshToken", refreshToken, _signInService.SetCookieOptions(TypesToken.Refresh));

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
        return Ok(new ErrorsResponse(400, "Token not found."));

      string email = _signOutService.TokenDecryption(token);
      if (string.IsNullOrEmpty(email))
        return Ok(new ErrorsResponse(400, "Token not found."));

      bool remove = await _signOutService.RemoveToken(email, token);
      if (!remove)
        return Ok(new ErrorsResponse(500, "Server error."));

      HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });

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
      string? refreshToken = HttpContext.Request.Cookies["refreshToken"];
      if (string.IsNullOrEmpty(refreshToken))
      {
        HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
        HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
        return Unauthorized();
      }

      bool isValidData = _authSundryService.ValidationEmail(request.email);
      if (!isValidData)
      {
        HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
        HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
        return Unauthorized();
      }

      bool isImmutableToken = await _authSundryService.CheckImmutableToken(request.email, refreshToken);
      if (!isImmutableToken)
      {
        HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
        HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
        return Unauthorized();
      }

      bool tokenIsValid = await _authSundryService.ValidateToken(refreshToken);
      if (!tokenIsValid)
      {
        await _authSundryService.RemoveRefreshToken();
        HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
        HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
        return Unauthorized();
      }

      string accessToken = _authSundryService.GetJwtAccessToken();
      if (string.IsNullOrEmpty(accessToken))
      {
        HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
        HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });
        return Unauthorized();
      }

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
        return Ok(new ErrorsResponse(400, $"{isValidData.Field} is incorrect or null."));

      if (request.password == request.newPassword)
        return Ok(new ErrorsResponse(400, $"The old password is the same as the new one."));

      bool setData = await _authSundryService.SetDataUpdatePassword(request);
      if (!setData)
        return Ok(new ErrorsResponse(500, "Server error."));

      bool verifiPassword = _authSundryService.VerificationPassword(request.password);
      if (!verifiPassword)
        return Ok(new ErrorsResponse(400, "Incorrect password."));

      string newHash = _authSundryService.GetHash(request.newPassword);
      if (string.IsNullOrEmpty(newHash))
        return Ok(new ErrorsResponse(500, "Server error."));

      bool success = await _authSundryService.SaveNewPassword(newHash);
      if (!success)
        return Ok(new ErrorsResponse(500, "Server error."));

      bool isRemoved = await _authSundryService.RemoveRefreshToken();
      if (!isRemoved)
        return Ok(new ErrorsResponse(500, "Server error."));

      bool sendEmail = await _authSundryService.SendUpdatePasswordEmail();
      if (!sendEmail)
        return Ok(new ErrorsResponse(500, "Server error."));

      HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });

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
        return Ok(new ErrorsResponse(400, $"{isValidData.Field} is incorrect or null."));

      bool comparison = await _authRecoveryService.СomparisonRecoveryPassData();
      if (!comparison)
        return Ok(new ErrorsResponse(400, "Some data doesn't match."));

      string newPassword = _authRecoveryService.GetNewPassword(16);
      if (string.IsNullOrEmpty(newPassword))
        return Ok(new ErrorsResponse(500, "Server error."));

      bool sendEmail = await _authRecoveryService.SendRecoveryPassEmail(newPassword);
      if (!sendEmail)
        return Ok(new ErrorsResponse(500, "Server error."));

      bool savePass = await _authRecoveryService.SaveNewPassword(newPassword);
      if (!savePass)
        return Ok(new ErrorsResponse(500, "Server error."));

      bool isRemoved = await _authRecoveryService.RemoveRefreshToken();
      if (!isRemoved)
        return Ok(new ErrorsResponse(500, "Server error."));

      HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-30) });
      HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions { MaxAge = TimeSpan.FromMinutes(-1440) });

      return Ok();
    }
  }
}