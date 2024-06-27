using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.AuthServices
{
  public interface ISignOutService
  {
    Task<IActionResult> LogoutAsync(HttpContext httpContext);
  }
}