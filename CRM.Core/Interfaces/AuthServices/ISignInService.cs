using CRM.Core.Contracts.RestDto;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.AuthServices
{
  public interface ISignInService
  {
    Task<IActionResult> SignInAsync(HttpContext httpContext, SignInRequest request);
  }
}
