using CRM.Core.Contracts.RestDto;

using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.Controllers.Auth
{
  public interface IRegisterController
  {
    Task<IActionResult> Register(RegisterRequest request);
    Task<IActionResult> ConfirmRegister(string code);
  }
}