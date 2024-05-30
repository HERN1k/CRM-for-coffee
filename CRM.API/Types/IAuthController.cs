using CRM.Core.Contracts.RestDto;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Types
{
    public interface IAuthController
  {
    Task<IActionResult> Register(RegisterRequest request);
    Task<IActionResult> ConfirmRegister(string code);
    Task<IActionResult> SignIn(SignInRequest request);
    Task<IActionResult> Logout();
    Task<IActionResult> ChangeToken(ChangeTokenRequest request);
    Task<IActionResult> UpdatePassword(UpdatePasswordRequest request);
    Task<IActionResult> RecoveryPassword(RecoveryPasswordRequest request);
  }
}
