using CRM.Core.Contracts.RestDto;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.AuthServices
{
  public interface IAuthSundryService
  {
    Task<IActionResult> ChangeTokenAsync(HttpContext httpContext, ChangeTokenRequest request);
    Task<IActionResult> UpdatePasswordAsync(HttpContext httpContext, UpdatePasswordRequest request);
  }
}