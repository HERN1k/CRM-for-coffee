using CRM.Core.Contracts.RestDto;

using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.Controllers.Auth
{
  public interface IAuthSundryController
  {
    Task<IActionResult> ChangeToken(ChangeTokenRequest request);
    Task<IActionResult> UpdatePassword(UpdatePasswordRequest request);
  }
}