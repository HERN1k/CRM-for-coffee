using CRM.Core.Contracts.RestDto;

using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.AuthServices
{
  public interface IRegisterService
  {
    Task<IActionResult> RegisterNewWorkerAsync(RegisterRequest request);
    Task<IActionResult> ConfirmRegisterAsync(string code);
  }
}