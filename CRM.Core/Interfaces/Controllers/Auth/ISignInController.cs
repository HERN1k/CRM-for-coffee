using CRM.Core.Contracts.RestDto;

using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.Controllers.Auth
{
  public interface ISignInController
  {
    Task<IActionResult> SignIn(SignInRequest request);
  }
}