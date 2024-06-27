using CRM.Core.Contracts.RestDto;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.AuthServices
{
  public interface IAuthRecoveryService
  {
    Task<IActionResult> RecoveryPasswordAsync(HttpContext httpContext, RecoveryPasswordRequest request);
  }
}