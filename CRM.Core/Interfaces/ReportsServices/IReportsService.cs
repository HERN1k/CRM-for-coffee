using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.ReportsServices
{
  public interface IReportsService
  {
    Task<IActionResult> MenuAsync();
    Task<IActionResult> WorkersAsync();
    Task<IActionResult> OrdersAsync();
  }
}