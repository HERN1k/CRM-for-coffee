using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.Controllers.Auth
{
  public interface ISignOutController
  {
    Task<IActionResult> Logout();
  }
}